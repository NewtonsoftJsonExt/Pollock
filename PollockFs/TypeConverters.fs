namespace Pollock

open System
open System.Collections.Generic

module public TypeConverters = 
    let cs = new Dictionary<Type, ITypeConverter>()
    
    let register<'T> (converter : ITypeConverter) = 
        let t = typeof<'T>
        if (not(cs.ContainsKey(t))) then
            cs.Add(t, converter)
        else
            cs.[t] <- converter 
    
    let registerEnumWithMemberRender<'T when 'T : struct>() = register<'T> (new EnumWithMemberRenderConverter<'T>())
    
    let get<'T when 'T : struct>() : ITypeConverter = 
        let t = typeof<'T>
        if (not (cs.ContainsKey(t))) then registerEnumWithMemberRender<'T>() |> ignore
        else ()
        cs.[t]
