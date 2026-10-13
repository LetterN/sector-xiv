using Content.Shared._Citadel.Currency;

namespace Content.Server._Citadel.Currency;

/// <summary>
/// Server part of the economy, handles bank accounts (private)
/// </summary>
public sealed class EconomySystem : SharedEconomySystem
{
    private readonly List<BankAccount> _bankAccounts = new();

    public override void Initialize()
    {
        _bankAccounts.Add(BankAccount.StationBankAccount);
        BankAccount.StationBankAccount.AddCurrency(CurrencyType.CitadelStationCurrency.GetHashCode(), 25_000_000);
        _bankAccounts.Add(BankAccount.TerraGovAccount);
        BankAccount.TerraGovAccount.AddCurrency(CurrencyType.CitadelStationCurrency.GetHashCode(), 25_000_000);
        _bankAccounts.Add(BankAccount.SyndicateAccount);
        BankAccount.SyndicateAccount.AddCurrency(CurrencyType.SyndicateCurrency.GetHashCode(), 25_000_000);
        BankAccount.SyndicateAccount.AddCurrency(CurrencyType.CitadelStationCurrency.GetHashCode(), 1_000_000);
        // TODO saveload things here for bank accounts
        base.Initialize();
    }

    public void CreateBankAccount(string name, string password)
    {
        _bankAccounts.Add(new BankAccount(name, password));
    }

    /// <summary>
    /// Get bank account
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password">leave blank for nonstrict lookup (name only)</param>
    /// <returns></returns>
    public BankAccount? GetBankAccount(string name, string? password)
    {
        foreach (var bankAccount in _bankAccounts)
        {
            if (bankAccount.Name == name && (password == null || bankAccount.Password == password))
            {
                return bankAccount;
            }
        }

        return null;
    }
}
