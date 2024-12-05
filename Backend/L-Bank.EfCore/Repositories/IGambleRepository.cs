namespace L_Bank.EfCore.Repositories;

public interface IGambleRepository
{
    bool Gamble(int userId, decimal amount);
}