namespace Content.Shared._Citadel.Currency;

/// <summary>
/// A type of currency.
/// </summary>
[DataRecord]
public readonly partial struct CurrencyType : IEquatable<CurrencyType>
{
    public static CurrencyType CitadelStationCurrency = new("Citadel Credits", "CRD", '₡');
    public static CurrencyType SyndicateCurrency = new("Telecrystals", "TC", 'ℸ');
    public static CurrencyType TestStationCurrency = new(
        "Really Broken Money With A Stupidly Long Length To Test UIs and Other Things",
        "TEST",
        'T');

    /// <summary>
    /// Full name of the currency, ie `Spacebux`
    /// </summary>
    [DataField]
    public readonly string Name;

    /// <summary>
    /// Shortname of the currency, ie `BUX`. Limited to 4 chars
    /// </summary>
    [DataField]
    public readonly string Shortname;

    /// <summary>
    /// Char/symbol of the currency, ie `$`
    /// </summary>
    [DataField]
    public readonly char Symbol; // should this be really a symbol or an icon *shrug

    // public readonly string Format; // like 0.00{sym} or {sym}00.00 idk

    public CurrencyType(string name, string shortname, char symbol)
    {
        Name = name;
        Shortname = shortname.Substring(0, Math.Min(shortname.Length, 4));
        Symbol = symbol;
    }

    public bool Equals(CurrencyType other)
    {
        return Name == other.Name && Shortname == other.Shortname && Symbol == other.Symbol;
    }

    public override bool Equals(object? obj)
    {
        return obj is CurrencyType other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Shortname, Symbol);
    }
}

[DataRecord]
public readonly partial struct BankAccount : IEquatable<BankAccount>
{
    public static BankAccount StationBankAccount = new("Citadel Station Accounting", adminbuse: true);
    public static BankAccount TerraGovAccount = new("TerraGov Finances", adminbuse: true);
    public static BankAccount SyndicateAccount = new("Donk Co. Account C", adminbuse: true);

    // maybe account id for uniqueness?
    [DataField]
    public readonly string Name;
    [DataField]
    public readonly string Password;

    /// TODO: flag this OR split enum it: ADMIN_ACCOUNT | (STATION_ACCOUNT | PLAYER_ACCOUNT | COMPANY_ACCOUNT)
    public readonly bool Private = false;

    // public readonly EntityUid? OwnerRef; // eugh, need a way to ref this to a player when server restarts
    /// <summary>
    /// id = CurrencyType hash, value = count
    /// </summary>
    public readonly Dictionary<int, float> AccountBalance;

    public readonly List<BankRecord> BankRecords;

    public BankAccount(string name, string password = "", bool adminbuse = false)
    {
        Name = name;
        Password = password;
        Private = adminbuse;
        AccountBalance = new Dictionary<int, float>();
        BankRecords = [new BankRecord(CurrencyType.CitadelStationCurrency.GetHashCode(), note: "Account Created")];
    }

    public static bool TransferCurrency(BankAccount from, BankAccount to, int currency, float value, string? note)
    {
        if (from.AccountBalance[currency] < value)
        {
            // too poor
            return false;
        }

        from.AddCurrency(currency, -value, "Transfer to " + to.Name + " Note:" + note);
        from.AddCurrency(currency, value, "Transfer from " + from.Name + " Note:" + note);
        return true;
    }

    /// <summary>
    /// Add currency. You can also remove currency by setting the value to negatives.
    /// This one handles "logging" of currency transfer (BankRecords)
    /// </summary>
    /// <param name="currency">hash of the currency</param>
    /// <param name="value"></param>
    /// <param name="note"></param>
    public void AddCurrency(int currency, float value, string note = "")
    {
        AccountBalance[currency] += value;
        BankRecords.Add(new BankRecord(currency, value, note));
    }

    public bool Equals(BankAccount other)
    {
        return Name == other.Name && Password == other.Password && Private == other.Private;
    }

    public override bool Equals(object? obj)
    {
        return obj is BankAccount other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Password);
    }
}

/// <summary>
/// A record of transactional events on an account
/// </summary>
[DataRecord]
public readonly partial struct BankRecord(int currencyType, float currencyAmount = 0f, string note = "")
{
    /// <summary>
    /// Note for this specific transaction
    /// </summary>
    public readonly string Note = note;
    /// <summary>
    /// Currency dabbled in this transaction
    /// </summary>
    public readonly int CurrencyType = currencyType;
    /// <summary>
    /// How much was transfered/given (-/+)
    /// </summary>
    public readonly float CurrencyAmount = currencyAmount;

    /// <summary>
    /// IC timestamp on when this happened. or maybe ooc time?
    /// </summary>
    public readonly DateTime WorldTimestamp = DateTime.UtcNow;
}
