namespace DoorstepCO.Core.Validation;

/// <summary>
/// Plausible ranges for the numbers of an address. Values outside them are reported as issues, not rejected.
/// </summary>
internal static class AddressLimits
{
    public const int MaxPlateNumber = 150;
}
