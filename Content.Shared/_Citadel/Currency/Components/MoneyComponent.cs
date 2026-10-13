using Robust.Shared.GameStates;

namespace Content.Shared._Citadel.Currency.Components;

/// <summary>
/// Indicates that this entity IS a physical representation of money, not its value! ATM eats this
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MoneyComponent : Component
{
    /// <summary>
    /// Hash ID of the currency type. we are not storing what actual currency is this here lmao
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrencyType = Currency.CurrencyType.TestStationCurrency.GetHashCode();

    [DataField, AutoNetworkedField]
    public float Amount = 0;
}
