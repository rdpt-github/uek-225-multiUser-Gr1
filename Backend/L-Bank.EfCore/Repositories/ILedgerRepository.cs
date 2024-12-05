using L_Bank_W_Backend.Core.Models;

namespace L_Bank.EfCore.Repositories;

public interface ILedgerRepository
{
    IEnumerable<Ledger> GetAllLedgers();
    public string Book(decimal amount, Ledger from, Ledger to);
    decimal GetTotalMoney();
    Ledger? SelectOne(int id);
    void Update(Ledger ledger);
    decimal? GetBalance(int ledgerId);
    Ledger Create(Ledger ledger);
}