namespace L_Bank.EfCore.Repositories;

public class MoneyPrinterRepository(LBankDbContext dbContext) : IMoneyPrinterRepository
{
    public bool Add100(int ledgerId)
    {
        bool worked;
        do
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var ledger = dbContext.Ledgers.FirstOrDefault(l => l.Id == ledgerId);
                ledger!.Balance += 100;
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
}