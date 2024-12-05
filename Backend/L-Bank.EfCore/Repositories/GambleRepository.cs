namespace L_Bank.EfCore.Repositories;

public class GambleRepository(LBankDbContext dbContext) : IGambleRepository
{
    public bool Gamble(int ledgerId, decimal amount)
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

        return worked;
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