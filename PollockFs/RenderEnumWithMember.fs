namespace Pollock

open System
open System.Collections.Generic
open System.Linq
open System.Runtime.Serialization

type EnumNameMapping<'T when 'T : struct>(typeToString : IDictionary<'T, string>) = 
    
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

type EnumWithMemberRenderConverter<'T when 'T : struct>() = 
    let t = typeof<'T>
    
    let GetMemberValue(v : 'T) : string = 
        let memInfo = t.GetMember(v.ToString())
        let enumMember = typeof<EnumMemberAttribute>
        let attributes = memInfo.[0].GetCustomAttributes(enumMember, false).Cast<EnumMemberAttribute>()
        if (attributes.Any()) then attributes.First().Value
        else v.ToString()
    
    let enumValues = Enum.GetValues(t).Cast<'T>()
    let typeToStringMapping = enumValues.ToDictionary((fun v -> v), (fun v -> GetMemberValue(v)))
    let mapping = new EnumNameMapping<'T>(typeToStringMapping)
    interface ITypeConverter with
        member this.ToString(culture, value) = mapping.ToString(value :?> 'T)
        member this.FromString(culture, value) = mapping.Parse(value) :> obj
