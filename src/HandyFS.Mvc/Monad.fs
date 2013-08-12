namespace HandyFS.Mvc

///Contains functions for chaining together functions which make up a controller action
[<AutoOpen>]
module Monad =

    open System
    open System.Web.Mvc

    ///HTTP status code used for errors
    let [<Literal>] ErrorCode = 500

    ///Lifts a value to the form used by the libraruy
    let lift x =
        fun _ ->
            x

    ///Lifts a unary function to the form used by the libraryy
    let liftfn f = 
        fun value (_ : Controller) ->
            f value

    ///Starting function for function chains (return)
    let with' value = 
        lift (Continue value)

    ///Binds two functions together, calling the second if the first does not break. The second function accepts the controller as an argument.
    let (+>>) f g = 
        fun (controller : Controller) ->
            match (f controller) with
            | Break resultType -> Break resultType
            | Continue value -> g value controller

    ///Binds two functions together, calling the second if the first does not break. The second function does not accept the controller as an argument.
    let (->>) f g = 
        f +>> (liftfn g)

    ///Binds two functions together to form the end of the action. The second function accepts the controller as an argument. An HTTP 500 response
    ///is generated if the second function does not break.
    let (+<>) f g = 
        fun (controller : Controller) ->

            let resultType = 
                match (f controller) with
                | Break resultType' -> resultType'
                | Continue value ->
                    match (g value controller) with
                    | Break resultType' -> resultType'
                    | Continue _ -> Status ErrorCode

            let createResult = 
                match resultType with
                | Status statusCode -> lift (Result.isStatusCode statusCode)
                | View model -> Result.isView model
                | Redirect redirectType -> 
                    match redirectType with
                    | Url url -> lift (Result.isUrlRedirect url)
                    | Route data -> lift (Result.isRouteRedirect data)

            createResult controller

    ///Binds two functions together to form the end of the action. The second function does not accept the controller as an argument. An HTTP 500 response
    ///is generated if the second function does not break.
    let (-<>) f g = 
        f +<> (liftfn g)

