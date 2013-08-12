namespace HandyFS.Mvc.Example.Models

open System

type CreateUserModel () = 
    member val UserName = String.Empty with get, set
    member val Password = String.Empty with get, set

type UserCreatedModel (userName : String) = 
    member val UserName = userName with get, set

