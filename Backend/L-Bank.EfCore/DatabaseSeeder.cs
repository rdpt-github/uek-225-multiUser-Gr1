using L_Bank_W_Backend.Core.Models;
using L_Bank.Core.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace L_Bank.EfCore;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly DatabaseSettings databaseSettings;

    public DatabaseSeeder(IOptions<DatabaseSettings> databaseSettings)
    {
        this.databaseSettings = databaseSettings.Value;
    }

    public void Initialize()
    {
        // CreateDatabaseIfNotExists();
        // CreateTablesIfNotExists();
    }

    public void Seed()
    {
        SeedLedgers();
        SeedUsers();
    }

    private void CreateDatabaseIfNotExists()
    {
        var createDbQuery =
            $@"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseSettings.DatabaseName}') CREATE DATABASE {databaseSettings.DatabaseName}";
        using (var masterConnection = new SqlConnection(databaseSettings.MasterConnectionString))
        {
            var command = new SqlCommand(createDbQuery, masterConnection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private void CreateTablesIfNotExists()
    {
        const string createLedgersDbQuery =
            @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ledgers' and xtype='U')
                BEGIN
                    CREATE TABLE ledgers (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        name nvarchar(50) NOT NULL,
                        balance money NOT NULL
                    )
                END";
        using (var connection = new SqlConnection(databaseSettings.ConnectionString))
        {
            var command = new SqlCommand(createLedgersDbQuery, connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }

        const string createUsersDbQuery =
            @$"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{User.CollectionName}' and xtype='U')
                BEGIN
                    CREATE TABLE users (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        username nvarchar(50) NOT NULL,
                        role nvarchar(50) NOT NULL,
                        password_hash nvarchar(60) NOT NULL,
                    )
                END";
        using (var connection = new SqlConnection(databaseSettings.ConnectionString))
        {
            var command = new SqlCommand(createUsersDbQuery, connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private bool IsEmpty(string tableName)
    {
        var query = $"SELECT COUNT(*) FROM {tableName}";

        var count = 0;
        using (var conn = new SqlConnection(databaseSettings.ConnectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        return count == 0;
    }


    private void SeedLedgers()
    {
        if (!IsEmpty(Ledger.CollectionName)) return;

        var moneyProvider = new Random();

        var seedLedgers = new List<Dictionary<string, object>>
        {
            new()
            {
                { "name", "Manitu AG" },
                { "balance", moneyProvider.Next(100, 10001) }
            },
            new()
            {
                { "name", "Chrysalkis GmbH" },
                { "balance", moneyProvider.Next(100, 10001) }
            },
            new()
            {
                { "name", "Smith & Co KG" },
                { "balance", moneyProvider.Next(100, 10001) }
            }
        };

        using (var conn = new SqlConnection(databaseSettings.ConnectionString))
        {
            conn.Open();

            foreach (var data in seedLedgers)
            {
                const string query = "INSERT INTO ledgers (name, balance) VALUES (@Name, @Balance)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", data["name"]);
                    cmd.Parameters.AddWithValue("@Balance", data["balance"]);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    private void SeedUsers()
    {
        if (!IsEmpty("users")) return;

        var moneyProvider = new Random();

        var seedUsers = new List<Dictionary<string, string>>
        {
            new()
            {
                { "username", "admin" },
                { "password_hash", PasswordHelper.HashAndSaltPassword("adminpass") },
                { "role", Roles.Administrators.ToString() }
            },
            new()
            {
                { "username", "testuser" },
                { "password_hash", PasswordHelper.HashAndSaltPassword("testuserpass") },
                { "role", Roles.Users.ToString() }
            }
        };

        using (var conn = new SqlConnection(databaseSettings.ConnectionString))
        {
            conn.Open();

            foreach (var data in seedUsers)
            {
                const string query =
                    "INSERT INTO users (username, PasswordHash, role) VALUES (@Username, @PasswordHash, @Role)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", data["username"]);
                    cmd.Parameters.AddWithValue("@PasswordHash", data["password_hash"]);
                    cmd.Parameters.AddWithValue("@Role", data["role"]);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}