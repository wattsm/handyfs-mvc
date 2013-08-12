namespace HandyFS.Mvc.Example.Controllers

open System
open System.Web.Mvc
open HandyFS.Mvc
open HandyFS.Mvc.Example.Models

[<AbstractClass>]
type BaseController () = 
    inherit Controller () 

    member this.Run f = 
        f this

module Users = 
    
    let private _reservedNames = [ "admin"; ]
    let private _existingNames = [ "jsmith"; "jdoe"; ]

    [<AutoOpen>]
    module private Helpers = 

        let containsUserName (model : CreateUserModel) = 
            List.exists (fun userName ->
                model.UserName.Equals (userName, StringComparison.OrdinalIgnoreCase)
            )       

        let addGeneralError (message : String) (controller : Controller) =
            controller.ModelState.AddModelError (String.Empty, message)

        let addPropertyError (propertyName : String) (message : String) (controller : Controller) = 
            controller.ModelState.AddModelError (propertyName, message)

    let checkForReservedName (model : CreateUserModel) = 

        let isReserved = 
            _reservedNames
            |> containsUserName model

        if isReserved then
            breakWithStatus 400
        else
            continueWith model

    let checkForDuplicateName (model : CreateUserModel) = 
        fun controller -> 

            let isDuplicate = 
                _existingNames
                |> containsUserName model

            if isDuplicate then

                controller
                |> addPropertyError "UserName" "User name already in use."

                breakWithView model
            else
                continueWith model

    let createUser (model : CreateUserModel) = 
        
        //Some business logic to create the user here

        let route = 
            {
                Name = "User.Confirm";
                Values = [ ("userName", box model.UserName); ];
            }

        breakWithRedirect (Route route)

type UserController () = 
    inherit BaseController ()

    member this.Form () = 
        this.View (CreateUserModel ())

    [<HttpPost>]
    member this.Form (model : CreateUserModel) = 
        this.Run (
            with' model
            ->> Users.checkForReservedName
            +>> Users.checkForDuplicateName
            -<> Users.createUser
        )

    member this.Confirm (userName : String) = 
        this.View (UserCreatedModel (userName))
