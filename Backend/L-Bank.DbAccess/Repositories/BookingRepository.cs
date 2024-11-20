using System.Data.SqlClient;
using System.Data;
using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.DbAccess;
using Microsoft.Extensions.Options;


namespace L_Bank_W_Backend.Models
{
    public class BookingRepository : IBookingRepository
    {
        DatabaseSettings settings;
        public BookingRepository(IOptions<DatabaseSettings> settings)
        {
            this.settings = settings.Value;
        }
        
        public bool Book(int sourceLedgerId, int destinationLKedgerId, decimal amount)
        {
            // Machen Sie eine Connection und eine Transaktion

            // In der Transaktion:

            // Schauen Sie ob genügend Geld beim Spender da ist
            // Führen Sie die Buchung durch und UPDATEn Sie die ledgers
            // Beenden Sie die Transaktion
            // Bei einem Transaktionsproblem: Restarten Sie die Transaktion in einer Schleife 
            // (Siehe LedgersModel.SelectOne)

            return false; // Lösch mich
        }
        
        public decimal GetTotalMoney()
        {
            const string query = @$"SELECT SUM(balance) AS TotalBalance FROM {Ledger.CollectionName}";
            decimal totalBalance = 0;

            using (SqlConnection conn = new SqlConnection(this.settings.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        totalBalance = Convert.ToDecimal(result);
                    }
                }
            }

            return totalBalance;
        }


    }
}

