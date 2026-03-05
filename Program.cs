var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options=>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseCors("FrontendPolicy");



app.Use(async (context, next) =>
{
    /*Console.WriteLine($"Method: {context.Request.Method}");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"QueryString: {context.Request.QueryString}");
    Console.WriteLine(string.Join("\n", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}")));
    Console.WriteLine($"IP: {context.Connection.RemoteIpAddress}");*/

    // if(context.Request.Path == "/api/movie")
    // {
    //     context.Response.StatusCode = 403;
    //     await context.Response.WriteAsync("Access Denied");
    //     return;
    // }

    // if (!context.Request.Query.ContainsKey("key") )
    // {
    //     context.Response.StatusCode = 401;
    //     await context.Response.WriteAsync("Key missing");
    //     return;
    // }
    if (!context.Request.Path.StartsWithSegments("/.well-known"))
    {
        string log = $"Request: {context.Request.Method} {context.Request.Path}\n";
        await File.AppendAllTextAsync("logs.txt", log);
    }
    await next();
});


app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();



app.Run();
