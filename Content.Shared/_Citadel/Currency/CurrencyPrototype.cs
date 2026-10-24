using Robust.Shared.Prototypes;

namespace Content.Shared._Citadel.Currency;

[Prototype]
public sealed partial class CurrencyPrototype : IPrototype
{
    [IdDataField, ViewVariables(VVAccess.ReadOnly)]
    public string ID { get; private set; } = default!;

    [DataField(required: true), ViewVariables]
    public string Name = string.Empty;

    [DataField, ViewVariables]
    public string Shortname = string.Empty;

    [DataField, ViewVariables]
    public char Symbol = ' ';
}
