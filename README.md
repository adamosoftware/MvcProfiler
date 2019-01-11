# Background

This is a profiler for MVC5 (.NET Framework, not Core) that captures execution times for MVC actions. I got interested in this suddenly because, well, I'm not quite sure. Sometimes I get suddenly interested in trying something because I think I will need it, and because I think it's a useful exercise. I did look first at [MiniProfiler](https://miniprofiler.com). I'm sure their product is excellent, but they have a different definition of "simple" than I do. A few things to note about my approach at the outset:

- I don't have a good testing approach, and I'm really not sure how accurate my recorded executing times are. Profiling can get complicated, but I've deliberately tried to stick to a "common sense" approach. I'm not clear how well my solution will work with things like partial views and other subtleties of the MVC pipeline.

- There's no UI in my approach. I merely capture run times and some related HTTP request data in some tables. I would love to incorporate Razor UI in my package, but I'm not sure how to make that work from a Nuget package.

- There are two packages in my solution: **MvcProfiler.Library** and **MvcProfiler.Postulate**. The Library version has no database nor ORM dependency. You must implement several abstract methods to provide database integration. The Postulate version uses [Postulate.Lite.SqlServer](https://github.com/adamosoftware/Postulate.Lite) because this would be my common use case.

- The profile data captured is [here](https://github.com/adamosoftware/MvcProfiler/tree/master/MvcProfiler/Models). There are three model classes: [Request](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/Models/Request.cs) is the main thing. One Request may have related [Query string parameters](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/Models/Parameter.cs) as well as any number of intermediate [steps](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/Models/Step.cs).

## Usage

- If you want full control over how profile data is captured with no ORM or database dependency, install package **MvcProfiler.Library** and write your own class based on [ProfilerBase](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/ProfilerBase.cs) and implement its three abstract methods: [VerifyDatabaseObjects](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/ProfilerBase.cs#L53), [Save](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/ProfilerBase.cs#L59), and [GetConnection](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/ProfilerBase.cs#L64).

- If you want to use Postulate.Lite ORM and SQL Server, installed package **MvcProfiler.Postulate**. You must still write your own class based on [PostulateProfilerBase](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler.Postulate/PostulateProfilerBase.cs), and implement the [GetConnection](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler/ProfilerBase.cs#L64) abstract method.

- I have a sample project [here](https://github.com/adamosoftware/MvcSpace) that uses MvcProfiler.Postulate. These are the key points of it:

    - Create class [Profiler](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/Profiler.cs) based on `PostulateProfilerBase`. There's just one abstract method to implement `GetConnection`. In this example, it uses my standard connection method. You will likely have your own typical way of opening a connection.
    
    - In the app [startup](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/Startup.cs#L14) I call `VerifyDatabaseObjects` to ensure that the [model class tables](https://github.com/adamosoftware/MvcProfiler/tree/master/MvcProfiler/Models) are built. The table creation work is done [here](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler.Postulate/PostulateProfilerBase.cs#L21)
    
    - In my [BaseController](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs) that serves as the template for all my controllers in thie app, I do several things:
    
    - Create a `Profiler` [property](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L17)
    
    - Initialize the `Profiler` property during the Controller `Initialize` override [here](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L28). Notice that the current `requestContext` ius passed.
    
    - In the `Dispose` override for the controller, I [Stop](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L80) the profiler, which causes it to save to the database. This is the Postulate-specific [Save](https://github.com/adamosoftware/MvcProfiler/blob/master/MvcProfiler.Postulate/PostulateProfilerBase.cs#L40) implementation.
    
    - You will likely want to profile certain queries or other intermediate steps in your controllers. I have several -- one to mark the time for [loading user profile info](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L35), and I specifically record whether the profile came from the cache or the database. I also record some [query times](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/Controllers/DataModelController.cs#L20) in a specific controller. Finally, I record the HTML rendering time with [StepBegin](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L45) and [StepEnd](https://github.com/adamosoftware/MvcSpace/blob/master/MvcSpace.App/BaseController.cs#L74), which requires the `OnActionExecuting` and `OnActionExecuted` overrides, respectively.
