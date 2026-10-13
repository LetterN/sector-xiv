using Robust.Shared.Utility;

namespace Content.Shared._Citadel.Currency;

/// <summary>
/// This is where you register currency types. Shared both client & server
/// </summary>
public abstract class SharedEconomySystem : CitadelSystem
{
    private readonly Dictionary<int, CurrencyType> _currencyTypes = new();

    public override void Initialize()
    {
        _currencyTypes.Add(CurrencyType.CitadelStationCurrency.GetHashCode(), CurrencyType.CitadelStationCurrency);
        _currencyTypes.Add(CurrencyType.TestStationCurrency.GetHashCode(), CurrencyType.SyndicateCurrency);
        _currencyTypes.Add(CurrencyType.TestStationCurrency.GetHashCode(), CurrencyType.TestStationCurrency);
        base.Initialize();
    }

    public void RegisterCurrency(string name, string shortname, char symbol)
    {
        var ct = new CurrencyType(name, shortname, symbol);
        if (_currencyTypes.ContainsKey(ct.GetHashCode()))
        {
            return;
        }
        _currencyTypes.Add(ct.GetHashCode(), ct);
    }

    public CurrencyType? GetCurrencyByName(string name)
    {
        foreach (var ct in _currencyTypes.Values)
        {
            if (ct.Name == name)
            {
                return ct;
            }
        }
        return null;
    }

    public CurrencyType? GetCurrencyByShortname(string shortname)
    {
        foreach (var ct in _currencyTypes.Values)
        {
            if (ct.Shortname == shortname)
            {
                return ct;
            }
        }
        return null;
    }
}
