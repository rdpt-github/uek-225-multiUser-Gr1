namespace L_Bank_W_Backend.DbAccess.Repositories;

public interface IGambleRepository
{
    bool Gamble(int userId, decimal amount);
}