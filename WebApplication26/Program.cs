var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<Backend>(c => c.BaseAddress = new("http://backend"));

var app = builder.Build();

app.MapGet("/", (Backend client) => client.HelloAsync());

app.Run();

class Backend(HttpClient client)
{
    public Task<string> HelloAsync() => client.GetStringAsync("/hello");
}