namespace DoorstepCO.Core;

/// <summary>
/// Kind of problem detected while validating an address.
/// </summary>
public enum IssueCode
{
    /// <summary>The input is empty or contains only whitespace.</summary>
    Empty,

    /// <summary>The input could not be parsed as an urban address.</summary>
    Unparseable,
}
