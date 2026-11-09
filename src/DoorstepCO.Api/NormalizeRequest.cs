namespace DoorstepCO.Api;

/// <summary>
/// Body of a request to normalize a single address.
/// </summary>
/// <param name="Address">The raw address text, e.g. "cll 59 # 80-12 apto 302".</param>
public sealed record NormalizeRequest(string? Address);
