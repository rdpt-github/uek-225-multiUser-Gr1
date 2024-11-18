namespace L_Bank_W_Backend.Models;

public interface IBookingService
{
    bool Book(int sourceLedgerId, int destinationLKedgerId, decimal amount);
}