namespace Pollock

open System.ComponentModel
open System.Collections.Generic
open System.Linq
open System
open System.Runtime.Serialization

type EnumNameMapping<'T when 'T : struct>(?typeToStringOpt : IDictionary<'T, string>) = 
    let t = typeof<'T>
    let getMemberValue(v : 'T) : string = 
        let memInfo = t.GetMember(v.ToString())
        let enumMember = typeof<EnumMemberAttribute>
        let attribute = memInfo.[0].GetCustomAttributes(enumMember, false).Cast<EnumMemberAttribute>() 
                        |> Seq.tryHead
        match attribute with
        | Some a -> a.Value
        | None -> v.ToString()

    let typeToString = 
        match typeToStringOpt with
        | Some v-> v
        | None -> Enum.GetValues(t).Cast<'T>()
                    .ToDictionary((fun v -> v), (fun v -> getMemberValue(v))) :> IDictionary<'T, string>

    let stringToType = 
            typeToString.ToArray()
            |> Array.map (fun kv -> (kv.Value, kv.Key))
            |> Map.ofSeq

    member this.ParseAsStandardEnum(value : string) : 'T = Enum.Parse(typeof<'T>, value, true) :?> 'T
    
    member this.Parse(v : string) : 'T = 
        if (String.IsNullOrEmpty(v)) then Unchecked.defaultof<'T>
        else if (stringToType.ContainsKey(v)) then stringToType.[v]
        else this.ParseAsStandardEnum(v)
    
    member this.ToString(value : 'T) : string = 
        if (not (typeToString.ContainsKey(value))) then value.ToString()
        else typeToString.[value]

type public EnumTypeConverter<'T when 'T : struct>() = 
    inherit TypeConverter()
    let mapping = new EnumNameMapping<'T>()
    
    override this.CanConvertFrom(context, sourceType) = 
        if (sourceType = typeof<string>) then true
        else base.CanConvertFrom(context, sourceType)
    
    override this.ConvertFrom(context, culture, value) = 
        if (value :? string) then mapping.Parse(value :?> string) :> obj
        else base.ConvertFrom(context, culture, value)
    
    override this.ConvertTo(context, culture, value, destinationType) = 
        if (destinationType = typeof<string>) then mapping.ToString(value :?> 'T) :> obj
        else base.ConvertTo(context, culture, value, destinationType)

