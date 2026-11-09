using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DoorstepCO.Api.Tests;

public sealed class BatchEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BatchEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostNormalizeBatch_CsvFile_ReturnsNormalizedCsvAttachment()
    {
        using var content = Upload("pedidos.csv", Encoding.UTF8.GetBytes("ID;DIRECCION\n1;cll 1 # 2-3\n"));

        var response = await _client.PostAsync("/normalize/batch", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition!.FileNameStar.Should().Be("pedidos-normalizada.csv");
        var body = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
        body.Should().Contain("ID;DIRECCION;NORMALIZADA\r\n1;cll 1 # 2-3;CL 1 2 3\r\n");
    }

    [Theory]
    [InlineData("pedidos.csv", "ID;CIUDAD\n1;Cali\n", "MissingAddressColumn")]
    [InlineData("pedidos.csv", "", "EmptyFile")]
    [InlineData("pedidos.pdf", "DIRECCION\n", "UnsupportedFileType")]
    [InlineData("pedidos.xlsx", "DIRECCION\n", "UnreadableFile")]
    public async Task PostNormalizeBatch_InvalidFile_ReturnsBadRequestWithErrorCode(
        string fileName, string text, string expectedCode)
    {
        using var content = Upload(fileName, Encoding.UTF8.GetBytes(text));

        var response = await _client.PostAsync("/normalize/batch", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("code").GetString().Should().Be(expectedCode);
    }

    [Fact]
    public async Task PostNormalizeBatch_FileTooLarge_ReturnsBadRequestWithErrorCode()
    {
        using var content = Upload("pedidos.csv", new byte[(10 * 1024 * 1024) + 1]);

        var response = await _client.PostAsync("/normalize/batch", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("code").GetString().Should().Be("FileTooLarge");
    }

    [Fact]
    public async Task PostNormalizeBatch_NoFile_ReturnsBadRequest()
    {
        using var content = new MultipartFormDataContent();

        var response = await _client.PostAsync("/normalize/batch", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static MultipartFormDataContent Upload(string fileName, byte[] bytes)
    {
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return new MultipartFormDataContent { { file, "file", fileName } };
    }
}
