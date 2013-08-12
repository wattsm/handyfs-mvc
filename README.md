### HandyFS.Mvc - F# MVC helper library

When writing ASP.Net MVC controllers in F# I often find myself creating nesting or branching code like the following:

```fsharp
[<HttpPost>]
member this.Form (model : CreateUserModel) = 
  if (Users.isReservedName model) then
    //HTTP 400
    
  else if (Users.isDuplicateName model) then
    //Display error
    
  else
    //Create user
    //Redirect
    
    
```

I also found that I was doing a lot of casting to ``ActionResult`` to keep F#'s type system happy (e.g. ``this.View (model) :> ActionResult``) in actions with multiple outcome types.

This leads to hard to read code and makes unit testing tricky, so I created a small set of monadic functions for chaining together functions which make up
an F# MVC controller action. This allows you to write code which is easier to read and easier to unit test, and also avoids the need for unnecessary casting. The functions
work in a similar way to the maybe monad.

```fsharp
[<HttpPost>]
member this.Form (model : CreateUserModel) = 
  this.Run (
    with' model
    ->> Users.checkForReservedName
    +>> Users.checkForDuplicateName
    -<> Users.createUser
  )
```

The ``with'`` function sets the initial state of the chain of functions. Functions on the right hand side of ``->>`` have signature ``'a -> Instruction<'b>`` where ``'a`` is the type of the value
returned by the previous function in the chain, and ``Instruction<'b>`` is either a break or a continue command. If a function returns a break command any subsequent functions are not called. Functions
on the right hand side of ``+>>`` simply accept the executing Controller as an additional argument, giving them signature ``'a -> Controller -> Instruction<'b>``.

The ``-<>`` (and ``+<>``) functions denote the end of the chain and it is here that resulting ``ActionResult`` is calculated. If the function on the right hand side of ``-<>`` or ``+<>`` does not
return a break command then an ``HttpStatusCodeResult`` of 500 is returned.

The example project ``HandyFS.Mvc.Example`` shows the functions in action for a trivial example. Note the ``Run`` member on the ``BaseController`` type executes the chained function, passing itself
as the argument.