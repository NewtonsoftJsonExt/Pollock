namespace Pollock

open System.Globalization

type public ITypeConverter = 
    abstract FromString : CultureInfo * string -> obj
    abstract ToString : CultureInfo * obj -> string
