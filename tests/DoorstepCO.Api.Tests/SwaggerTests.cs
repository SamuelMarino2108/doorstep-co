using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DoorstepCO.Api.Tests;

public sealed class SwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SwaggerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder => builder.UseEnvironment("Production"))
            .CreateClient();
    }

    [Theory]
    [InlineData("/swagger/v1/swagger.json", "application/json")]
    [InlineData("/swagger/index.html", "text/html")]
    public async Task GetSwagger_InProduction_ReturnsDocument(string path, string expectedMediaType)
    {
        var response = await _client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be(expectedMediaType);
    }
}
