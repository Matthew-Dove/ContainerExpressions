# ContainerExpressions
Containers for types, and expressions for those containers, enabling code to have fewer branching conditions.  
  
[If you prefer slides, there is some documentation here too.](https://docs.google.com/presentation/d/1ma8E9YohW2clB_lrB0cklm2_AelkBrBcl2f9DMwu8T4/edit?usp=sharing)  
[PM> Install-Package ContainerExpressions](https://www.nuget.org/packages/ContainerExpressions/)  

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

Note: there is also Try.RunAsync() for asynchronous functions.

### Either`<T>`

If you have a function that can benefit from returning one type, from a selection of types, then Either is what you're looking for.  
Either can change it's internal type during program execution similar to object, but in a type safe way. 

A case where you might use Either is when you have a function, and you find yourself about to drastically change it's return type for an edge case.  
Let's say you have a SaveCustomer function that takes a Customer, and returns a boolean indicating if the save was successful or not.  
A new requirement comes in, you must display a error message to the client if the customer's email is already in use by another client.  
You can't just rely on the boolean's false, because you don't know if it was false from of a write error, or a duplicate email.  
So you must change the return type from boolean to CustomerResponse, and CustomerResponse contains two fields, one for the boolean, and another string for the error message.  
Alternatively you could modify the return type to be `Either<bool, string>`, and return a string in the case of a duplicate email.  

In the example below we display different messages based off Either's internal type.  
Here we see types used to indicate state, something that would normally be done with values of a type, not the type itself.  
````cs
// Given the two types:
struct Ok { }
struct Error { }

// We can set Either to contain one of these two types.
Either<Ok, Error> either = new Ok();

if (new Random().Next() % 2 == 0)
{
	either = new Error();
}

string message = either.Match(
	ok => "Operation was successful.", // When Either's type is Ok, this string is returned.
	error => "Internal error - try again later." // When Either's type is Error, this string is returned.
);
````

### Trace

Logs function inputs, and outputs so you can save them to a trace file.  

Tracing is necessary in any non-trivial program to determine production runtime bugs.  
However these traces typically get in the way of the core code, and force you to break up code into pieces so you can log it's current state.  
The trace container slots in with existing code by taking a type, and returning that same type.  

Before you use Trace you must set a logger, an Action that takes a string (returns void), see below for an example.  

`Trace.SetLogger(Console.WriteLine);`  

See below for an example of logging the output of each function:  

````cs
// Initialize the Trace with a logger.
var logs = new List<string>();
Trace.SetLogger(log => logs.Add(log));

// Create a function to trace the incrementing.
Func<int, string> trace = x => string.Format("The value of the int is {0}.", x);

// Some functions that keep incrementing their input.
Func<Response<int>> identity = () => Response.Create(0);
Func<int, Response<int>> increment = x => Response.Create(x + 1);

var count = Expression.Compose(identity.Log(trace), increment.Log(trace), increment.Log(trace));

// The follow is logged to Trace:
// The value of the int is 0.
// The value of the int is 1.
// The value of the int is 2.
````

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

### Retry`<T>`

Execute the same function until it's Response is valid, or you run out of retries as defined by the options.  
By default the options are set to 1 retry, and a delay of 100 milliseconds before trying again.  
There is a method overload to pass in your own options for a more customized Retry.  

In the example below, we create a user in a database, and get their Id in return.

```cs
// Using default options, this will try a second time if the first time fails.
var userId = Retry.Execute(() => CreateUser(new UserModel { Name = "John Smith" }));

public Response<T> CreateUser(UserModel user)
{
	var response = new Response<int>(); // Invalid state, indicates a Retry.
	
	try
	{
		using (var connection = new SqlConnection(connectionString))
		using (var command = new SqlCommand("usp_insert_createUser", connection))
		{
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.Add("@Name" user.Name);
			connection.Open();
			
			var userId = (int)command.ExecuteScalar();
			response = response.WithValue(userId); // State is now valid, no need to run the function again.
		}
	}
	catch (exception ex)
	{
		// Log error here...
	}
	
	return response;
}
```

### Reduce`<T>`

Combine many values of `T` to a single value of `T`.  
Useful for any type that is associative.  
Only valid states of `Response<T>` are combined in the reduce expression.  

In the example below we combine many words into a sentence.  

```cs
Func<string, string, string> combine = (x, y) => string.Concat(x, " ", y);

var words = new Response<string>[] { Response.Create("world") };
var arg1 = "hello";

string sentence = Expression.Reduce(combine, arg1, words);
```

### Funnel`<T>`

Takes many `Response<T>`'s and invokes a function passing each `T` as an argument; if and only if each `Response<T>` is in a valid state.  
If at least one input is in an invalid state, then an invalid `Response<T>` is returned instead of calling the function.  
Useful when you'd like an operation to happen only when several previous operation completed successfully.  

In the example below we only take the power of two numbers if they were calculated correctly.  

```cs
// Returns an Invalid Response<double> when the divisor argument is 0, otherwise the result is stored in a valid Response<double>.
Func<double, double, Response<double>> divide = (dividend, divisor) => divisor == 0 ? new Response<double>() : Response.Create(dividend / divisor);

var e = divide(150D, 55D);
var pi = divide(22D, 7D);

var answer = Expression.Funnel(e, pi, Math.Pow);
```

## Extension Methods

Useful utilities for the `Response<T>` type.  

* `T GetValueOrDefault<T>(T defaultValue)` Returns the default value when the Response is in an invalid state.
* `Response<TResult> Bind<TResult>(Func<T, Response<TResult>> func)` Invokes a second function with the output of the first one.
* `Response<TResult> Transform<T, TResult>(Func<T, TResult> func)` Changes type `T`, to type `TResult`.
* `Func<T, Response<TResult>> Lift<T, TResult>(Func<T, TResult>)` Elevate the function’s type from `T`, to `Response<T>`
* `Response<TResult> Pivot<T, TResult>` Execute the first function if the condition is true, otherwise execute the second function.  
* `bool IsTrue<T>` When the Response is in a valid state the func's bool result is returned, otherwise false is returned.  