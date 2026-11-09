using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DoorstepCO.Api.Tests;

public sealed class WebUiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WebUiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoot_WhenCalled_ReturnsIndexPage()
    {
        var response = await _client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
        (await response.Content.ReadAsStringAsync()).Should().Contain("<title>Doorstep CO</title>");
    }

    [Theory]
    [InlineData("/styles.css", "text/css")]
    [InlineData("/app.js", "text/javascript")]
    public async Task GetAsset_WhenCalled_ReturnsFile(string path, string expectedMediaType)
    {
        var response = await _client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be(expectedMediaType);
    }
}
