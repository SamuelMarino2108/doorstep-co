using System.Text.Json.Serialization;
using DoorstepCO.Api;
using DoorstepCO.Api.Batch;
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

app.MapPost("/normalize/batch", async (IFormFile file, CancellationToken cancellationToken) =>
    {
        try
        {
            if (file.Length > BatchFile.MaxSizeInBytes)
            {
                throw new BatchFileException(BatchError.FileTooLarge);
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            var result = BatchFile.Normalize(file.FileName, stream.ToArray());
            return Results.File(result.Content, result.ContentType, result.FileName);
        }
        catch (BatchFileException exception)
        {
            return Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?> { ["code"] = exception.Error.ToString() });
        }
    })
    .WithName("NormalizeBatch")
    .WithTags("Normalize")
    .WithDescription("Upload a .csv or .xlsx file with a DIRECCION column; returns the same file with a "
        + "NORMALIZADA column appended.")
    .DisableAntiforgery()
    .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
    .ProducesProblem(StatusCodes.Status400BadRequest);

app.Run();

/// <summary>
/// Entry point of the API. Declared as a public partial class so integration tests can reference it.
/// </summary>
public partial class Program;
