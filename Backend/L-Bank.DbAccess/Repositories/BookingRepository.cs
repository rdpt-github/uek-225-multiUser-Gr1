using System.Data;
using System.Data.SqlClient;
using L_Bank_W_Backend.Models;
using Microsoft.Extensions.Options;

namespace L_Bank_W_Backend.DbAccess.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        DatabaseSettings settings;
        public BookingRepository(IOptions<DatabaseSettings> settings)
        {
            this.settings = settings.Value;
        }

        public bool Book(int sourceLedgerId, int destinationLedgerId, decimal amount)
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
                            var sourceBalance = GetLedgerBalance(connection, transaction, sourceLedgerId);
                            if (sourceBalance < amount)
                            {
                                return false;
                            }

                            UpdateLedgerBalance(connection, transaction, sourceLedgerId, -amount);
                            Thread.Sleep(2000);
                            UpdateLedgerBalance(connection, transaction, destinationLedgerId, amount);

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

        private decimal GetLedgerBalance(SqlConnection connection, SqlTransaction transaction, int ledgerId)
        {
            using (var command = new SqlCommand("SELECT Balance FROM Ledgers WHERE id = @LedgerId", connection, transaction))
            {
                command.Parameters.AddWithValue("@LedgerId", ledgerId);
                return (decimal)command.ExecuteScalar();
            }
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
}

