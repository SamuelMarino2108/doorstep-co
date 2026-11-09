using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DoorstepCO.Api.Tests;

public sealed class NormalizeEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NormalizeEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostNormalize_ValidAddress_ReturnsNormalizedAddress()
    {
        var response = await _client.PostAsJsonAsync("/normalize", new { Address = "cll 59 a bis sur # 80-12 apto 302" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("normalized").GetString().Should().Be("CL 59A BIS SUR 80 12 AP 302");
        body.RootElement.GetProperty("confidence").GetDouble().Should().Be(1.0);
        body.RootElement.GetProperty("issues").GetArrayLength().Should().Be(0);
        body.RootElement.GetProperty("address").GetProperty("roadType").GetString().Should().Be("Calle");
    }

    [Fact]
    public async Task PostNormalize_UnparseableAddress_ReturnsIssueCodeAsText()
    {
        var response = await _client.PostAsJsonAsync("/normalize", new { Address = "frente al parque" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("normalized").ValueKind.Should().Be(JsonValueKind.Null);
        body.RootElement.GetProperty("issues")[0].GetProperty("code").GetString().Should().Be("Unparseable");
    }

    [Fact]
    public async Task PostNormalize_UnknownNamedAvenue_ReturnsRoadName()
    {
        var response = await _client.PostAsJsonAsync("/normalize", new { Address = "av el poblado # 30-28" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("address").GetProperty("road").GetProperty("name").GetString()
            .Should().Be("EL POBLADO");
        body.RootElement.GetProperty("issues")[0].GetProperty("code").GetString().Should().Be("NamedRoad");
    }

    [Fact]
    public async Task PostNormalize_MissingAddress_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/normalize", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
