using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class RoadLettersTests
{
    [Theory]
    [InlineData("a", "A")]
    [InlineData("b", "B")]
    [InlineData("z", "Z")]
    [InlineData("aa", "AA")]
    [InlineData("ae", "AE")]
    [InlineData("ab", "AB")]
    [InlineData("as", "AS")]
    public void TryGetLetter_OneOrTwoLetters_ReturnsUppercaseLetters(string token, string expected)
    {
        var found = RoadLetters.TryGetLetter(token, out var letter);

        found.Should().BeTrue();
        letter.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("n")]
    [InlineData("s")]
    [InlineData("e")]
    [InlineData("no")]
    [InlineData("abc")]
    [InlineData("bis")]
    [InlineData("5")]
    [InlineData("#")]
    public void TryGetLetter_OtherToken_ReturnsFalse(string token)
    {
        var found = RoadLetters.TryGetLetter(token, out _);

        found.Should().BeFalse();
    }
}
