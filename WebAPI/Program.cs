using Microsoft.Net.Http.Headers;
using TRIAS.NET.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("TriasClient", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("TRIAS.NET:Endpoint") ?? "");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "TRIAS.NET/0.0.1");
});
builder.Services.AddTransient<ILocationService, LocationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
