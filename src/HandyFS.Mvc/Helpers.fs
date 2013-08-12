namespace HandyFS.Mvc

///Contains helper types and functions
[<AutoOpen>]
module Helpers =

    open System    

    ///Break with an HTTP status code
    let breakWithStatus statusCode = 
        Break (Status statusCode)

    ///Break and display a model
    let breakWithView model = 
        Break (View model)

    ///Break and redirect
    let breakWithRedirect redirect = 
        Break (Redirect redirect)

    ///Continue with a value
    let continueWith value = 
        Continue value

    ///Contains functions for creating results
    [<RequireQualifiedAccess>]
    module Result = 

        open System.Web.Mvc
        open System.Web.Routing

        ///Casts a result as ActionResult
        let asAction<'TResult when 'TResult :> ActionResult> (result : 'TResult) = 
            result :> ActionResult

        ///Creates a view result for the given model
        let isView<'TModel> (model : 'TModel) = 
            fun (controller : Controller) ->

                //TODO Copied across all apparently relevant information from the controller, does anything else need to be set?

                let setViewData (view : ViewResult) = 
                    view.ViewData <- controller.ViewData
                    view

                let setViewEngine (view : ViewResult) = 
                    view.ViewEngineCollection <- controller.ViewEngineCollection
                    view

                let setModel (view : ViewResult) = 
                    view.ViewData.Model <- model
                    view

                ViewResult ()
                |> setViewData
                |> setViewEngine
                |> setModel
                |> asAction

        ///Creates a status code result with the given HTTP status code
        let isStatusCode (code : Int32) = 
            HttpStatusCodeResult (code)
            |> asAction

        ///Creates a redirect to route result
        let isRouteRedirect data = 
    
            let routeValues = 
                RouteValueDictionary (
                    data.Values
                    |> dict
                )  
        
            RedirectToRouteResult (data.Name, routeValues)
            |> asAction 

        ///Creates a redirect to URL result
        let isUrlRedirect (url : String) = 
            RedirectResult (url)
            |> asAction    
