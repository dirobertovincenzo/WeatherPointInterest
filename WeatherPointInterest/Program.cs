using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Headers;
using WeatherPointInterest.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("Weather", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WeatherBaseUrl"));
    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

});
builder.Services.AddHttpClient("Business", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BusinessBaseUrl"));
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //"vwCUDDoUaABrpUUoVUXCltwuYsPGKrGy5Hu5E9G_QuK3HHsw0-NRu9EPd8luHt6Gbvv6wQtukLtauHstHKoeZ7WdwjOcNuSuCwG7Pg0BFZbxtQIhBYudK_YfNr5KY3Yx"
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",builder.Configuration.GetValue<string>("BusinessAPIKey"));


});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
