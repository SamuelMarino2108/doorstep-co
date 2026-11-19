using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class OrdinalKeywordsTests
{
    [Theory]
    [InlineData("ra")]
    [InlineData("ro")]
    [InlineData("era")]
    [InlineData("ero")]
    [InlineData("er")]
    [InlineData("da")]
    [InlineData("do")]
    [InlineData("ta")]
    [InlineData("to")]
    public void IsOrdinalSuffix_Suffix_ReturnsTrue(string token)
    {
        OrdinalKeywords.IsOrdinalSuffix(token).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("o")]
    [InlineData("aa")]
    [InlineData("no")]
    public void IsOrdinalSuffix_OtherToken_ReturnsFalse(string token)
    {
        OrdinalKeywords.IsOrdinalSuffix(token).Should().BeFalse();
    }

    [Theory]
    [InlineData("primera", 1)]
    [InlineData("primero", 1)]
    [InlineData("segunda", 2)]
    [InlineData("tercera", 3)]
    [InlineData("cuarta", 4)]
    [InlineData("quinta", 5)]
    [InlineData("sexta", 6)]
    [InlineData("septima", 7)]
    [InlineData("octava", 8)]
    [InlineData("novena", 9)]
    [InlineData("decima", 10)]
    [InlineData("decimo", 10)]
    public void TryGetNumber_OrdinalWord_ReturnsNumber(string word, int expected)
    {
        var found = OrdinalKeywords.TryGetNumber(word, out var number);

        found.Should().BeTrue();
        number.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("once")]
    [InlineData("calle")]
    public void TryGetNumber_OtherToken_ReturnsFalse(string word)
    {
        OrdinalKeywords.TryGetNumber(word, out _).Should().BeFalse();
    }
}
