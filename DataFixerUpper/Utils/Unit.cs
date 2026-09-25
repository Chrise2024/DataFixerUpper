namespace DataFixerUpper.Utils;

/// <summary>
/// Unit for single value.
/// </summary>
public sealed class Unit
{
    /// <summary>
    /// Gets the single instance of the <see cref="Unit"/> class.
    /// </summary>
    public static Unit Instance => new();
    private Unit() { }
}