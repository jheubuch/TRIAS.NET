using Microsoft.Net.Http.Headers;
using TRIAS.NET.WebAPI.Helper;
using TRIAS.NET.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<TriasExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient("TriasClient", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("TRIAS.NET:Endpoint") ?? "");
    httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "TRIAS.NET/0.0.1");
});
builder.Services.AddTransient<ILocationService, LocationService>();
builder.Services.AddTransient<IBoardService, BoardService>();
builder.Services.AddTransient<IJourneyService, JourneyService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
