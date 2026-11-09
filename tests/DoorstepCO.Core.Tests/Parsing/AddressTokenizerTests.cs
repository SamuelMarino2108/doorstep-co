using DoorstepCO.Core.Parsing;

namespace DoorstepCO.Core.Tests.Parsing;

public sealed class AddressTokenizerTests
{
    [Theory]
    [InlineData("calle 59 # 80-12", "calle 59 # 80 - 12")]
    [InlineData("Cra. 7 No. 32-16", "cra 7 no 32 - 16")]
    [InlineData("CÁLLE 59", "calle 59")]
    [InlineData("cra7#32-16", "cra 7 # 32 - 16")]
    [InlineData("kr 7 n° 32 16", "kr 7 no 32 16")]
    [InlineData("Diagonal 40, Nº 20 - 10", "diagonal 40 no 20 - 10")]
    public void Tokenize_RawAddress_ReturnsNormalizedTokens(string input, string expectedTokens)
    {
        var tokens = AddressTokenizer.Tokenize(input);

        tokens.Should().Equal(expectedTokens.Split(' '));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(".,;")]
    public void Tokenize_NoMeaningfulCharacters_ReturnsEmpty(string input)
    {
        var tokens = AddressTokenizer.Tokenize(input);

        tokens.Should().BeEmpty();
    }
}
