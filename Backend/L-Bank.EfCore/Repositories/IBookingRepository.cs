namespace L_Bank.EfCore.Repositories;

public interface IBookingRepository
{
    bool Book(int sourceLedgerId, int destinationLKedgerId, decimal amount);
}