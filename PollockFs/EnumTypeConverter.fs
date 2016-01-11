namespace Pollock

open System.ComponentModel
open Newtonsoft.Json
open System.Collections.Generic
open System.Linq
open System
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

type public EnumTypeConverter<'T when 'T : struct>() = 
    inherit TypeConverter()
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
    
    override this.CanConvertFrom(context, sourceType) = 
        if (sourceType = typeof<string>) then true
        else base.CanConvertFrom(context, sourceType)
    
    override this.ConvertFrom(context, culture, value) = 
        if (value :? string) then mapping.Parse(value :?> string) :> obj
        else base.ConvertFrom(context, culture, value)
    
    override this.ConvertTo(context, culture, value, destinationType) = 
        if (destinationType = typeof<string>) then mapping.ToString(value :?> 'T) :> obj
        else base.ConvertTo(context, culture, value, destinationType)

type public EnumJsonConverter<'T when 'T : struct>() = 
    inherit JsonConverter()
    let c = TypeDescriptor.GetConverter(typeof<'T>)
    let ParseValue v = c.ConvertFrom(v)
    let ValueToString v = c.ConvertTo(v, typeof<string>)
    override this.CanConvert(objectType) = objectType = typeof<'T>
    
    override this.ReadJson(reader, objectType, existingValue, serializer) = 
        if (objectType = typeof<'T>) then 
            let v = serializer.Deserialize<string>(reader)
            ParseValue(v)
        else failwith ("Cant handle type")
    
    override this.WriteJson(writer, value, serializer) = 
        if (value :? 'T) then writer.WriteValue(ValueToString(value :?> 'T))
        else failwith ("not implemented")
