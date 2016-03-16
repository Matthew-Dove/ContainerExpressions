# ContainerExpressions
Containers for types, and expressions for those containers, enabling code to have fewer branching conditions.

## Containers

### Later`<T>`

Used in situations where you desire the value to be calulated the first time it's accessed.  
The value will only be calulated once, this container is thread safe.  

In the example below `IUserService` is injected into other services using [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection), the code runs in the context of a web server.  
So depending on what time the DI framework creates the `UserService`, and what time the user is authenticated (*and therefore sets the thread's `CurrentPrincipal`*), reading the name may cause an error.  
Using the later container we know longer have to care about the execution order of the authentication, and the dependency injector.
```cs
public class UserService : IUserService
{
	public string Name { get { return _username; } }
	private readonly Later<string> _username = null
    
    public UserService()
    {
    	_username = Later.Create(() => Thread.CurrentPrincipal.Identity.Name);
	}
}
```

### Response`<T>`

An enclosing type around a method's normal return type.  
Used when methods may return an error beyond your control, the container signals if the method ran successfully or not.
If the method successfully ran, you can access the *real* value, otherwise the program is in an error state.

In the example below we show the pattern for using the response container.
The response type will be valid only when a value is set, at least once.
```cs
public class CustomerService
{
    public Response<Customer> LoadCustomer(int id)
    {
        var response = new Response<Customer>(); // The response starts off in an invalid state.

        try
        {
            string json = File.ReadAllText($"~/Users/{id}.json");
            Customer customer = JsonConvert.DeserializeObject<Customer>(json);
            response = response.WithValue(customer); // The response is in a valid state.
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return response;
    }
}
```

The consuming code looks like:
```cs
CustomerService service = new CustomerService();
Response<Customer> customer service.LoadCustomer(1337);
if (customer.IsValid)
{
	// Do something with the customer...
}
```

### Response

Similar to Response`<T>`, but used for methods that return void, instead of a *real* type.
```cs
public class CustomerService
{
    public Response SaveCustomer(Customer customer)
    {
        var response = new Response(); // The response starts off in an invalid state.

        try
        {
            string json = JsonConvert.SerializeObject(customer);
            File.WriteAllText($"~/Users/{id}.json", json);
            response = response.AsValid(); // The response is in a valid state.
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        return response;
    }
}
```

## Expressions

### Compose`<T>`

Used to run dependant functions one after each other, such that the first function's output feeds into the second function's input.  
This continues until the last function, when that type is returned in the container `Response<T>`.
If any of the functions fail, the whole chain will fail and the final container's response will be invalid.

In the example below `Download`, `Unzip`, and `Persist` are all functions with the return type of `Response<T>`.  
The function `Download` retrieves a file from a server, the function `Unzip` decompresses the file, and the final function `Persist` saves data from that file to a database, returning the number of rows inserted. In this case the final function `Persist`, has a return type of `Response<int>`. If any one of these three functions fail, the end result of the expression will be an *invalid* response.
```cs
var rowsInserted = Expression.Compose<int>(Download, Unzip, Persist);
```
