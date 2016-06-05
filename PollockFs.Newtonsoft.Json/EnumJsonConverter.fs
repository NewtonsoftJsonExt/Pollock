namespace Pollock

open System.ComponentModel
open Newtonsoft.Json
open System.Collections.Generic
open System.Linq
open System
open System.Runtime.Serialization
open Pollock

type public EnumJsonConverter<'T when 'T : struct>() = 
    inherit JsonConverter()
    let mapping = new EnumNameMapping<'T>()

    override this.CanConvert(objectType) = objectType = typeof<'T>
    
    override this.ReadJson(reader, objectType, existingValue, serializer) = 
        if (objectType = typeof<'T>) then 
            let v = serializer.Deserialize<string>(reader)
            mapping.Parse(v) :> obj
        else failwith ("Cant handle type")
    
    override this.WriteJson(writer, value, serializer) = 
        if (value :? 'T) then writer.WriteValue(mapping.ToString(value :?> 'T))
        else failwith ("not implemented")
        