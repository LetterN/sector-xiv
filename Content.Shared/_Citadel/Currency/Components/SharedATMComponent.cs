using Robust.Shared.GameStates;

namespace Content.Shared._Citadel.Currency.Components;

/// <summary>
/// Component that simply tags something that it has an ATM ui.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SharedAtmComponent : Component
{
    /// <summary>
    /// Fluff ID for this atm. Displays on ui and withrawal logs*
    /// </summary>
    [DataField] public string AtmId;

    [DataField]
    public List<string> CurrencyAllowed;

    [DataField]
    public List<string> CurrencyBlocked;
}
