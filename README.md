# MvcProfiler

This is a profiler for MVC5 (.NET Framework, not Core) that captures execution times for MVC actions. I got interested in this suddenly because, well, I'm not quite sure. Sometimes I get suddenly interested in trying something because I think I will need it, and because I think it's a useful exercise. I did look first at [MiniProfiler](https://miniprofiler.com), but they have a different definition of "simple" than I do. A few things to note about my approach at the outset:

- I don't have a good testing approach, and I'm really not sure how accurate my recorded executing times are. Profiling can get complicated, but I've deliberately tried to stick to a "common sense" approach.

- There's no UI in my approach. I merely capture run times and some related HTTP request data in some tables. I would love to incorporate Razor UI in my package, but I'm not sure how to make that work from a Nuget package.

- There are two packages in my solution: **MvcProfiler.Library** and **MvcProfiler.Postulate**. The Library version has no database nor ORM dependency. You must implement several abstract methods to provide database integration. The Postulate version uses [Postulate.Lite.SqlServer](https://github.com/adamosoftware/Postulate.Lite) because this would be my common use case.

[to be continued... I'm still trying to fix Nuget packages]
