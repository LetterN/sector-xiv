using Content.Server.Stack;
using Content.Shared._Citadel.Currency;
using Content.Shared._Citadel.Currency.Components;

namespace Content.Server._Citadel.Currency;

/// <summary>
/// Server part of the economy, handles bank accounts (private)
/// </summary>
public sealed partial class EconomySystem : SharedEconomySystem
{
    [Dependency] private StackSystem _stack = default!;

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

    public BankAccount CreateBankAccount(string name, string password, bool adminbuse = false)
    {
        var bac = new BankAccount(name, password, adminbuse);
        _bankAccounts.Add(bac);
        return bac;
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

    /// <param name="atmRef">atm entity ref</param>
    /// <param name="currType">currency type hash</param>
    /// <param name="amount">Value to be withrawn, will be set negative!</param>
    /// <param name="name">account name</param>
    /// <param name="password">this is validated before being sent down here from the UI, only admins can null this</param>
    /// <param name="note"></param>
    public void WithrawFromAccount(
        EntityUid atmRef,
        int currType,
        float amount,
        string name,
        string? password,
        string note = "ATM #{MACHINE_ID} Withdrawal")
    {
        var account = GetBankAccount(name, password);
        if (!account.HasValue)
            return;
        amount = Math.Abs(amount);
        if (!account.Value.AddCurrency(currType, -amount, note))
            return;

        var money = SpawnAtPosition("money", Transform(atmRef).Coordinates);
        if (!TryComp<MoneyComponent>(money, out var moneyCmp))
            return;
        moneyCmp.CurrencyType = currType;
        moneyCmp.Amount = amount;
        _stack.TryMergeToContacts(money);
    }
}
