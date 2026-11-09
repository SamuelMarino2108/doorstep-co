using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class ComplementKeywordsTests
{
    [Theory]
    [InlineData("apartamento", ComplementType.Apartamento)]
    [InlineData("apto", ComplementType.Apartamento)]
    [InlineData("apt", ComplementType.Apartamento)]
    [InlineData("ap", ComplementType.Apartamento)]
    [InlineData("torre", ComplementType.Torre)]
    [InlineData("to", ComplementType.Torre)]
    [InlineData("t", ComplementType.Torre)]
    [InlineData("interior", ComplementType.Interior)]
    [InlineData("int", ComplementType.Interior)]
    [InlineData("in", ComplementType.Interior)]
    [InlineData("bloque", ComplementType.Bloque)]
    [InlineData("bloq", ComplementType.Bloque)]
    [InlineData("blq", ComplementType.Bloque)]
    [InlineData("bl", ComplementType.Bloque)]
    [InlineData("casa", ComplementType.Casa)]
    [InlineData("manzana", ComplementType.Manzana)]
    [InlineData("manz", ComplementType.Manzana)]
    [InlineData("mza", ComplementType.Manzana)]
    [InlineData("mz", ComplementType.Manzana)]
    [InlineData("lote", ComplementType.Lote)]
    [InlineData("lt", ComplementType.Lote)]
    [InlineData("conjunto", ComplementType.Conjunto)]
    [InlineData("conj", ComplementType.Conjunto)]
    [InlineData("cj", ComplementType.Conjunto)]
    public void TryGetComplementType_KnownKeyword_ReturnsType(string keyword, ComplementType expected)
    {
        var found = ComplementKeywords.TryGetComplementType(keyword, out var type);

        found.Should().BeTrue();
        type.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("ca")]
    [InlineData("cs")]
    [InlineData("con")]
    [InlineData("calle")]
    [InlineData("residencial")]
    public void TryGetComplementType_UnknownKeyword_ReturnsFalse(string keyword)
    {
        var found = ComplementKeywords.TryGetComplementType(keyword, out _);

        found.Should().BeFalse();
    }

    [Theory]
    [InlineData("residencial")]
    [InlineData("res")]
    public void IsResidential_ResidentialKeyword_ReturnsTrue(string keyword)
    {
        ComplementKeywords.IsResidential(keyword).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("conjunto")]
    [InlineData("r")]
    public void IsResidential_OtherKeyword_ReturnsFalse(string keyword)
    {
        ComplementKeywords.IsResidential(keyword).Should().BeFalse();
    }
}
