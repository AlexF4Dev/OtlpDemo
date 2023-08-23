var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapGet("/hello", async () =>
{
    await Task.Delay(Random.Shared.Next(500, 5000));
    return "Hello World!";
});

app.Run();
