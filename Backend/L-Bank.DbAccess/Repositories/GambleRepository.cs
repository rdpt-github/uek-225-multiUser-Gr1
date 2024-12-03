using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace L_Bank_W_Backend.DbAccess.Repositories;

public class GambleRepository
{
    DatabaseSettings settings;
    public GambleRepository(IOptions<DatabaseSettings> settings)
    {
        this.settings = settings.Value;
    }
    
    public bool Gamble(int ledgerId, decimal amount)
    {
        bool worked;
        do
        {
            using (var connection = new SqlConnection(settings.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        UpdateLedgerBalance(connection, transaction, ledgerId, amount);
                        Thread.Sleep(2000);
                        transaction.Commit();
                        worked = true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        worked = false;
                    }
                }
            }
        } while (!worked);

        return worked;
    }
    
    private void UpdateLedgerBalance(SqlConnection connection, SqlTransaction transaction, int ledgerId, decimal amount)
    {
        using (var command = new SqlCommand("UPDATE Ledgers SET Balance = Balance + @Amount WHERE id = @LedgerId", connection, transaction))
        {
            command.Parameters.AddWithValue("@LedgerId", ledgerId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.ExecuteNonQuery();
        }
    }
}