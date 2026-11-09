var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { Status = "ok" }))
    .WithName("GetHealth")
    .WithTags("Health");

app.Run();

/// <summary>
/// Entry point of the API. Declared as a public partial class so integration tests can reference it.
/// </summary>
public partial class Program;
