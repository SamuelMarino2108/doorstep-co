using DoorstepCO.Core.Normalization;

namespace DoorstepCO.Core.Tests.Normalization;

public sealed class RoadLettersTests
{
    [Theory]
    [InlineData("a", 'A')]
    [InlineData("b", 'B')]
    [InlineData("z", 'Z')]
    public void TryGetLetter_SingleLetter_ReturnsUppercaseLetter(string token, char expected)
    {
        var found = RoadLetters.TryGetLetter(token, out var letter);

        found.Should().BeTrue();
        letter.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("n")]
    [InlineData("ab")]
    [InlineData("5")]
    [InlineData("#")]
    public void TryGetLetter_OtherToken_ReturnsFalse(string token)
    {
        var found = RoadLetters.TryGetLetter(token, out _);

        found.Should().BeFalse();
    }
}
