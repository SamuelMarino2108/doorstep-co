using System.Text.Json.Serialization;
using DoorstepCO.Api;
using DoorstepCO.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { Status = "ok" }))
    .WithName("GetHealth")
    .WithTags("Health");

app.MapPost("/normalize", (NormalizeRequest request) =>
        request.Address is null
            ? Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(NormalizeRequest.Address)] = ["The address is required."],
            })
            : Results.Ok(AddressValidator.Validate(request.Address)))
    .WithName("Normalize")
    .WithTags("Normalize")
    .Produces<ValidationResult>()
    .ProducesValidationProblem();

app.Run();

/// <summary>
/// Entry point of the API. Declared as a public partial class so integration tests can reference it.
/// </summary>
public partial class Program;
