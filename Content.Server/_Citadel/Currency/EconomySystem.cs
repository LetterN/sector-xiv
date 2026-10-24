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

    private readonly HashSet<BankAccount> _bankAccounts = [];

    public override void Initialize()
    {
        base.Initialize();
        var stationBankAcc =
            CreateBankAccount("Citadel Station Accounting", type: BankAccount.BankAccountType.StationAccount);
        stationBankAcc.AddCurrency("cit_station_money", 25_000_000, note: "Account start");

        var terraGovAccount =
            CreateBankAccount("TerraGov Finances", type: BankAccount.BankAccountType.FactionAccount);
        terraGovAccount.AddCurrency("cit_station_money", 25_000_000, note: "Account start");

        var antagAccount =
            CreateBankAccount("Donk Co. Account C", type: BankAccount.BankAccountType.FactionAccount);
        antagAccount.AddCurrency("telecrystal", 25_000_000, note: "Account start");
        antagAccount.AddCurrency("cit_station_money", 1_000_000, note: "Account start");

        // TODO saveload things here for bank accounts
    }

    public BankAccount CreateBankAccount(string name, string? password = null, BankAccount.BankAccountType type = BankAccount.BankAccountType.CharacterAccount, string? parent = null)
    {
        var bac = new BankAccount(Guid.NewGuid().ToString(), name, password, type, parent);
        _bankAccounts.Add(bac);
        return bac;
    }

    /// <summary>
    /// Get bank account
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BankAccount? GetBankAccountByName(string name)
    {
        foreach (var bankAccount in _bankAccounts)
        {
            if (bankAccount.Name == name)
            {
                return bankAccount;
            }
        }
        return null;
    }

    public BankAccount? GetBankAccountByID(string guid)
    {
        foreach (var bankAccount in _bankAccounts)
        {
            if (bankAccount.AccountId == guid)
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
        string currType,
        float amount,
        string name,
        string? password,
        string note = "ATM #%MACHINE_ID% Withdrawal")
    {
        if (!TryComp<SharedAtmComponent>(atmRef, out var atmCmp))
            return;
        var account = GetBankAccountByName(name);
        if (!account.HasValue)
            return;
        amount = Math.Abs(amount);
        note = note.Replace("%MACHINE_ID%", atmCmp.AtmId);
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
