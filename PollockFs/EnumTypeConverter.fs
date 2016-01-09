namespace Pollock
open System.ComponentModel
open Newtonsoft.Json
open System.Globalization

type public EnumTypeConverter<'T when 'T : struct>() = 
    inherit TypeConverter()
    let _converter = TypeConverters.get<'T>()

    override this.CanConvertFrom (context, sourceType)=
        if (sourceType = typeof<string>) then
            true
        else
            base.CanConvertFrom(context, sourceType)

    override this.ConvertFrom (context,culture, value)=
        if (value :? string) then
            _converter.FromString(culture, (string)value)
        else
            base.ConvertFrom(context, culture, value)

    override this.ConvertTo(context,culture,value, destinationType)=
        if (destinationType = typeof<string>) then
            _converter.ToString(culture, value) :> obj
        else
            base.ConvertTo(context, culture, value, destinationType)

type public EnumJsonConverter<'T when 'T : struct> ()=
    inherit JsonConverter ()
    let c = TypeConverters.get<'T>()
    let ParseValue v =
        c.FromString(CultureInfo.CurrentCulture, v)
    let ValueToString v=
        c.ToString(CultureInfo.CurrentCulture, v)

    override this.CanConvert(objectType)=
        objectType = typeof<'T>;

    override this.ReadJson(reader, objectType, existingValue, serializer)=
            if (objectType = typeof<'T>) then
                let v = serializer.Deserialize<string>(reader)
                ParseValue(v);
            else
                failwith("Cant handle type");

    override this.WriteJson(writer, value, serializer)=
            if (value :? 'T) then
                writer.WriteValue(ValueToString(value :?> 'T));
            else
                failwith("not implemented");

