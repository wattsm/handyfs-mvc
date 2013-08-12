namespace HandyFS.Mvc

///Contains types used by the library
[<AutoOpen>]
module Types =

    open System

    ///Record containing the data required to redirect to a route
    type RouteData = {
        Name : String;
        Values : (String * obj) list;
    }

    ///Union describing possible redirect types
    type RedirectType = 
        | Url of String
        | Route of RouteData

    ///Union describing possible result types
    type ResultType = 
        | View of obj
        | Status of Int32
        | Redirect of RedirectType

    ///Union describing possible outcomes of the functions that make up a controller action
    type Instruction<'TValue> = 
        | Break of ResultType
        | Continue of 'TValue