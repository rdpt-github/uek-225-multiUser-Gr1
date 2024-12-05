namespace L_Bank.EfCore.Repositories;

public class BookingRepository(LBankDbContext dbContext) : IBookingRepository
{
    public bool Book(int sourceLedgerId, int destinationLedgerId, decimal amount)
    {
        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var sourceLedger = dbContext.Ledgers.FirstOrDefault(l => l.Id == sourceLedgerId);
                var destinationLedger = dbContext.Ledgers.FirstOrDefault(l => l.Id == destinationLedgerId);
                if (sourceLedger!.Balance < amount) return false;

                sourceLedger.Balance -= amount;
                dbContext.SaveChanges();

                destinationLedger!.Balance += amount;
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

        return worked;
    }

    private decimal GetLedgerBalance(int ledgerId)
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

    private void UpdateLedgerBalance(int ledgerId, decimal amount)
    {
        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var ledger = dbContext.Ledgers.FirstOrDefault(l => l.Id == ledgerId);
                ledger!.Balance += amount;
                dbContext.Ledgers.Update(ledger);
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
}