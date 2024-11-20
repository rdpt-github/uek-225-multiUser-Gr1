using System.Data;
using System.Data.SqlClient;
using L_Bank_W_Backend.Core.Models;
using Microsoft.Extensions.Options;

namespace L_Bank_W_Backend.DbAccess.Repositories;

public class LedgerRepository : ILedgerRepository
{
    private readonly DatabaseSettings databaseSettings;

    public LedgerRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        this.databaseSettings = databaseSettings.Value;
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        var allLedgers = new List<Ledger>();

        const string query = @$"SELECT id, name, balance FROM {Ledger.CollectionName} ORDER BY name";
        bool worked;
        do
        {
            worked = true;
            using (SqlConnection conn = new SqlConnection(this.databaseSettings.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                                    string name = reader.GetString(reader.GetOrdinal("name"));
                                    decimal balance = reader.GetDecimal(reader.GetOrdinal("balance"));

                                    allLedgers.Add(new Ledger { Id = id, Name = name, Balance = balance });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                        //Console.WriteLine("  Message: {0}", ex.Message);

                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            if (ex.GetType() != typeof(Exception))
                                worked = false;
                        }
                        catch (Exception ex2)
                        {
                            // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                            //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            //Console.WriteLine("  Message: {0}", ex2.Message);
                            if (ex2.GetType() != typeof(Exception))
                                worked = false;
                        }
                    }
                }
            }
        } while (!worked);

        return allLedgers;
    }
    
    public Ledger? SelectOne(int id)
    {
        Ledger? retLedger = null;
        bool worked;

        do
        {
            worked = true;
            using (SqlConnection conn = new SqlConnection(this.databaseSettings.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        retLedger = SelectOne(id, conn, transaction);
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                        //Console.WriteLine("  Message: {0}", ex.Message);

                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            if (ex.GetType() != typeof(Exception))
                                worked = false;
                        }
                        catch (Exception ex2)
                        {
                            // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                            //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            //Console.WriteLine("  Message: {0}", ex2.Message);
                            if (ex2.GetType() != typeof(Exception))
                                worked = false;
                        }
                    }
                }
            }
        } while (!worked);

        return retLedger;
    }
    
    public Ledger? SelectOne(int id, SqlConnection conn, SqlTransaction? transaction)
    {
        Ledger? retLedger;
        const string query = @$"SELECT id, name, balance FROM {Ledger.CollectionName} WHERE id=@Id";

        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    throw new Exception($"No Ledger with id {id}");

                int ordId = reader.GetInt32(reader.GetOrdinal("id"));
                string ordName = reader.GetString(reader.GetOrdinal("name"));
                decimal ordBalance = reader.GetDecimal(reader.GetOrdinal("balance"));

                retLedger = new Ledger { Id = ordId, Name = ordName, Balance = ordBalance };
            }
        }

        return retLedger;
    }
    
    public void Update(Ledger ledger, SqlConnection conn, SqlTransaction? transaction)
    {
        const string query = $"UPDATE {Ledger.CollectionName} SET name=@Name, balance=@Balance WHERE id=@Id";
        using (var cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Name", ledger.Name);
            cmd.Parameters.AddWithValue("@Balance", ledger.Balance);
            cmd.Parameters.AddWithValue("@Id", ledger.Id);

            // Execute the command
            cmd.ExecuteNonQuery();
        }
    }

    public void Update(Ledger ledger)
    {
        using (SqlConnection conn = new SqlConnection(this.databaseSettings.ConnectionString))
        {
            conn.Open();
            this.Update(ledger, conn, null);
        }
    }

}