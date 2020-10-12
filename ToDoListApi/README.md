A project following the tutorial at https://docs.microsoft.com/en-ca/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio-code.

What are different from the original example are this server adds logging and middleware.

To run it, do
```ps
$ dotnet restore
$ dotnet run
```

Then send a GET request to `localhost:5000/api/Ping` to see responses.
