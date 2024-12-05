namespace L_Bank.EfCore.Repositories;

public interface IMoneyPrinterRepository
{
    bool Add100(int ledgerId);
}