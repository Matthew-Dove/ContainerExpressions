# ContainerExpressions
Containers for types, and expressions for those containers, enabling code to have fewer branching conditions.

## Containers

### Response`<T>`

An enclosing type around a method's normal return type.  
Used when methods may return an error beyond your control, the container signals if the method ran successfully or not.
If the method completed successfully, you can access the *real* value, otherwise the response is in an error state.

In the example below we show the pattern for using the response container.
The response type will be valid only when a value is set.
```cs
class CustomerService
{
    public Response<Customer> LoadCustomer(int id)
    {
        var response = new Response<Customer>(); // The response starts off in an invalid state.

        try
        {
            string json = File.ReadAllText($"./User/{id}.json");
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
class CustomerService
{
    public Response SaveCustomer(int id, Customer customer)
    {
        var response = new Response(); // The response starts off in an invalid state.

        try
        {
            string json = JsonConvert.SerializeObject(customer);
            File.WriteAllText($"./Users/{id}.json", json);
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

### Later`<T>`

Used in situations where you desire the value to be calulated the first time it's accessed.  

In the example below `IUserService` is injected into other services using [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection), the code runs in the context of a web server.  
So depending on what time the DI framework creates the `UserService`, and what time the user is authenticated (*and therefore sets the thread's `CurrentPrincipal`*), reading the name may cause an error.  
Using the later container we no longer care about the execution order of the authentication, and the dependency injector.
```cs
class UserService : IUserService
{
	public string Name { get { return _username; } }
	private readonly Later<string> _username;
    
    public UserService()
    {
    	_username = Later.Create(() => Thread.CurrentPrincipal.Identity.Name);
	}
}
```
Note: there is also Later.CreateAsync() for asynchronous values.

### Try`<T>`

Wrap an `Action` (a void function), or a `Func<T>` (a function returning a "real" type) in a Try Container to safely execute otherwise problematic code.  
If the code in a function can throw errors, and those aren't handled internally the Try Container can help out.  
This would be used in cases where the function doesn't return a `Response`, or `Response<T>`, and can throw exceptions.  
It can make code clearer as the logic isn't clouded by error handling, however when to use this instead or handling the errors in the function itself is left to the implementer (you).  

By default errors aren't logged, but you can add your own logger that'll be ran each time the Try Container encounters an error.  
If you'd like to log any errors it's suggested you set up a logger at the start of the program, however you're able to change, or remove the error logger at any point in the program.  
Whatever logger is set at the time a Try container is created, is the logger that Container will use. It's suggested your logger is stateless to avoid runtime complications.  
The custom logger is a simple 'Action' that takes an 'Exception' as an argument. For example a logger in a console app might look like: `Try.SetExceptionLogger((ex) => Console.WriteLine(ex));`.  

In the example below a `Widget` is persisted to disk in a fire, and forget fashion.  
Since the result of the save isn't used, the return type is `void`. The function lacks error handling, so it's lifted to a Try Container.
````cs
var result = Try.Run(() => Persist(widget));

private static void Persist(Widget widget)
{
    var contents = JsonConvert.SerializeObject(widget);
    var path = $"{RELATIVE_PATH}/{Path.GetRandomFileName()}.json";
    File.WriteAllText(path, contents);
}
````

Note: there is also Try.CreateAsync() for asynchronous functions.

## Expressions

### Compose`<T>`

Used to run dependant functions one after each other, such that the first function's output feeds into the second function's input.  
This continues until the last function, when that type is returned in the container `Response<T>`.
If any of the functions fail, the whole chain will fail and the final container's response will be invalid.

In the example below `DownloadHtml`, and `PersistHtml` are functions with the return type of `Response<T>`, in this case T is string for both functions.  
The function `DownloadHtml` retrieves a webpage from a server, and the function `PersistHtml` saves that html to a file, returning the path to the new file.  
In this case the final function `PersistHtml`, has a return type of `Response<string>`, so the return type of `Expression.Compose`, will also be `Response<string>`.  
If either of these functions fail, the end result of the expression will be an invalid response.
```cs
var filepath = Expression.Compose(DownloadHtml, PersistHtml);
```

Note: there is also Expression.ComposeAsync() for composing asynchronous functions.

### Match`<T>`

Let's say you'd like to do different things based on the state of some input.  
For example summing an array of integers, you have three states, the array is null, the array is empty, the array has more then zero elements.  
You can have a pattern for each state (or a subset of the states), and have different behaviour for each pattern.  

In the example below when the input is null, we return an invalid response, when the input has no elements, we return 0, otherwise we return the sum of the array's elements.

```cs
var input = new int[] { 1, 2, 3 };

var result = Expression.Match(input,
	Pattern.Create<int[], int>(x => x == null, _ => Response.Create<int>()), // When null, return an invalid response.
	Pattern.Create<int[], int>(x => x.Length == 0, _ => Response.Create(0)), // When empty, return 0.
	Pattern.Create<int[], int>(x => x.Length > 0, Sum) // When more than zero elements exist, sum them, and return that result.
);

// Sum all elements in the array.
static Response<int> Sum(int[] numbers)
{
	var count = 0;
	for (int i = 0; i < numbers.Length; i++)
	{
		count += numbers[i];
	}
	return Response.Create(count);
}
```

Note: there is also Expression.MatchAsync() for asynchronous patterns.