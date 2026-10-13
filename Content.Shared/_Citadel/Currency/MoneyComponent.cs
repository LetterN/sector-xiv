namespace Content.Shared._Citadel.Currency;

/// <summary>
/// Indicates that this entity IS a physical representation of money, not its value! ATM eats this
/// </summary>
public sealed partial class MoneyComponent : Component
{
    /// <summary>
    /// Hash ID of the currency type. we are not storing what actual currency is this here lmao
    /// </summary>
    public int CurrencyType;
    public float Amount = 0;
}
