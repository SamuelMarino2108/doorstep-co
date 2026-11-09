namespace DoorstepCO.Core;

/// <summary>
/// Outcome of validating a free-text address.
/// </summary>
/// <param name="Address">The parsed address, or <see langword="null"/> when the input could not be parsed.</param>
/// <param name="Normalized">
/// The standard text form of the address (see <see cref="AddressFormatter.Format"/>), or <see langword="null"/>
/// when the input could not be parsed.
/// </param>
/// <param name="Confidence">
/// How likely the address is deliverable as written, from 0.0 (unusable) to 1.0 (no issues found).
/// </param>
/// <param name="Issues">Problems detected in the input, in a stable order.</param>
public sealed record ValidationResult(
    Address? Address,
    string? Normalized,
    double Confidence,
    IReadOnlyList<AddressIssue> Issues)
{
    /// <inheritdoc/>
    public bool Equals(ValidationResult? other) =>
        other is not null
        && Equals(Address, other.Address)
        && Normalized == other.Normalized
        && Confidence.Equals(other.Confidence)
        && Issues.SequenceEqual(other.Issues);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Address);
        hash.Add(Normalized);
        hash.Add(Confidence);
        foreach (var issue in Issues)
        {
            hash.Add(issue);
        }

        return hash.ToHashCode();
    }
}
