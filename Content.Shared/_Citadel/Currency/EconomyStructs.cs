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
    public enum BankAccountType
    {
        /// <summary>
        /// Magic account for arbitrage mechanics etc... not the wizard federation account. Admin only
        /// </summary>
        MagicAccount = 0,
        /// <summary>
        /// Account owned by a station entity. Admin only for now
        /// </summary>
        StationAccount = 1,
        /// <summary>
        /// Account owned by a player **character**
        /// </summary>
        PlayerAccount = 2,
        /// <summary>
        /// Account owned by a company entity
        /// </summary>
        CompanyAccount = 3,
        /// <summary>
        /// Account owned by a faction. Admin only
        /// </summary>
        FactionAccount = 4,
    }
    /// <summary>
    /// Unique account id. The value of this will be the hashmap AND the comparator for equality.
    /// </summary>
    [DataField]
    public readonly string AccountId;

    /// <summary>
    /// Account nickname
    /// </summary>
    [DataField]
    public readonly string Name;

    /// <summary>
    /// Account passwd, leave null for adminbus ones
    /// </summary>
    [DataField]
    public readonly string? Password;

    public readonly BankAccountType AccountType;

    public readonly string? ParentEntityID;

    /// <summary>
    /// id = CurrencyType hash, value = count
    /// </summary>
    public readonly Dictionary<int, float> AccountBalance;

    public readonly List<BankRecord> BankRecords;

    public BankAccount(string accountID, string name, string? password = null, BankAccountType accountType = BankAccountType.MagicAccount, string? parent = null)
    {
        AccountId = accountID;
        Name = name;
        Password = password;
        AccountType = accountType;
        ParentEntityID = parent;
        AccountBalance = new Dictionary<int, float>();
        BankRecords = [new BankRecord(CurrencyType.CitadelStationCurrency.GetHashCode(), note: "Account Created")];
    }

    /// <summary>
    /// Transfer currency from one account into another
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="currency"></param>
    /// <param name="value"></param>
    /// <param name="note"></param>
    /// <param name="force">allow the `from` account to go into debt</param>
    /// <returns>did the transaction complete?</returns>
    public static bool TransferCurrency(BankAccount from, BankAccount to, int currency, float value, string? note, bool force = false)
    {
        if (!from.AddCurrency(currency, -value, "Transfer to " + to.Name + " Note:" + note, force))
        {
            return false;
        }
        from.AddCurrency(currency, value, "Transfer from " + from.Name + " Note:" + note, force);
        return true;
    }

    /// <summary>
    /// Add currency. You can also remove currency by setting the value to negatives.
    /// This one handles "logging" of currency transfer (BankRecords)
    /// </summary>
    /// <param name="currency">hash of the currency</param>
    /// <param name="value"></param>
    /// <param name="note"></param>
    /// <param name="force">allow the account to go into negatives</param>
    public bool AddCurrency(int currency, float value, string note = "", bool force = false)
    {
        if (!force && value < 0 && AccountBalance[currency] < Math.Abs(value))
        {
            // too poor
            return false;
        }

        AccountBalance[currency] += value;
        BankRecords.Add(new BankRecord(currency, value, note));
        return true;
    }

    public bool Equals(BankAccount other)
    {
        return AccountId == other.AccountId;
    }

    public override bool Equals(object? obj)
    {
        return obj is BankAccount other && Equals(other);
    }

    public override int GetHashCode()
    {
        return AccountId.GetHashCode();
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
