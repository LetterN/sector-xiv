using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Citadel.Currency.Components;

/// <summary>
/// Indicates that this entity IS a physical representation of money, not its value! ATM eats this
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MoneyComponent : Component
{
    /// <summary>
    /// id of currency, internal ones are reasonably named, player ones are randomly named
    /// </summary>
    [DataField(customTypeSerializer: typeof(ProtoId<CurrencyPrototype>)), AutoNetworkedField]
    public string CurrencyType;

    [DataField, AutoNetworkedField]
    public float Amount = 0;
}
