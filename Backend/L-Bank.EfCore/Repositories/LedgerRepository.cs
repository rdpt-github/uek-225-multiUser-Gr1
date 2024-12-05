using L_Bank_W_Backend.Core.Models;

namespace L_Bank.EfCore.Repositories;

public class LedgerRepository(LBankDbContext dbContext) : ILedgerRepository
{
    public string Book(decimal amount, Ledger from, Ledger to)
    {
        using var transaction = dbContext.Database.BeginTransaction();
        try
        {
            var sourceLedger = dbContext.Ledgers.FirstOrDefault(l => l.Id == from.Id);
            var destinationLedger = dbContext.Ledgers.FirstOrDefault(l => l.Id == to.Id);
            if (sourceLedger!.Balance < amount) return "E";

            sourceLedger.Balance -= amount;
            dbContext.Ledgers.Update(sourceLedger);
            dbContext.SaveChanges();

            Thread.Sleep(2000);

            destinationLedger!.Balance += amount;
            dbContext.Ledgers.Update(destinationLedger);
            dbContext.SaveChanges();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            return "R";
        }

        return ".";
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        var allLedgers = new HashSet<Ledger>();

        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                allLedgers = dbContext.Ledgers.ToHashSet();
                transaction.Commit();
                worked = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                worked = false;
            }
        } while (!worked);

        return allLedgers;
    }

    public decimal GetTotalMoney()
    {
        var balance = 0m;

        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var allLedgers = dbContext.Ledgers;
                foreach (var ledger in allLedgers) balance += ledger.Balance;
                transaction.Commit();
                worked = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                worked = false;
            }
        } while (!worked);

        return balance;
    }

    public Ledger? SelectOne(int id)
    {
        Ledger? ledger = null;

        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                ledger = dbContext.Ledgers.FirstOrDefault(l => l.Id == id);
                transaction.Commit();
                worked = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                worked = false;
            }
        } while (!worked);

        return ledger;
    }

    public void Update(Ledger ledger)
    {
        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var loadedLedger = dbContext.Ledgers.FirstOrDefault(l => l.Id == ledger.Id);
                loadedLedger!.Balance = ledger.Balance;
                loadedLedger.Name = ledger.Name;
                dbContext.Ledgers.Update(loadedLedger);
                dbContext.SaveChanges();

                transaction.Commit();
                worked = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                worked = false;
            }
        } while (!worked);
    }


    public decimal? GetBalance(int ledgerId)
    {
        decimal ledgerBalance = 0;
        using var transaction = dbContext.Database.BeginTransaction();
        try
        {
            ledgerBalance = dbContext.Ledgers.FirstOrDefault(l => l.Id == ledgerId)!.Balance;
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }

        return ledgerBalance;
    }

    public Ledger Create(Ledger ledger)
    {
        if (ledger.Id == null || ledger.Name == null || ledger.Balance == null)
            throw new ArgumentException("Ledger has null Fields");

        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                ledger = dbContext.Ledgers.Add(ledger).Entity;
                dbContext.SaveChanges();

                transaction.Commit();
                worked = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                worked = false;
            }
        } while (!worked);

        return ledger;
    }
}