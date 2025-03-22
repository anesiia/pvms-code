using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using pvms_lb1_ClassLibrary_Chervenko;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

Class1 class1 = new Class1();

app.MapPost("/getHEX", async (HttpContext context) =>
{
    try
    {
        string requestBodyText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        Console.WriteLine($"Raw Request Body: {requestBodyText}");

        var requestBody = JsonSerializer.Deserialize<RequestModel>(requestBodyText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true 
        });

        if (requestBody?.Text == null)
        {
            return Results.BadRequest(new { error = "Invalid input: Text is required" });
        }

        Console.WriteLine($"Input: {requestBody.Text}");
        string result = class1.CalcEquation(requestBody.Text);
        Console.WriteLine($"Output: {result}");

        return Results.Json(new { result });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Server error: {ex.Message}");
        return Results.Problem($"Server error: {ex.Message}");
    }
});

app.Run("http://0.0.0.0:5000");

record RequestModel
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
