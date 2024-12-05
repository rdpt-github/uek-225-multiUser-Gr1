﻿using L_Bank_W_Backend.Core.Models;
using L_Bank.Core.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace L_Bank.EfCore.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseSettings settings;

    public UserRepository(IOptions<DatabaseSettings> settings)
    {
        this.settings = settings.Value;
    }

    public User? Authenticate(string? username, string? password)
    {
        User retUser;

        const string query =
            @$"SELECT id, username, passwordhash, role FROM {User.CollectionName} WHERE username=@Username";
        using (var conn = new SqlConnection(settings.ConnectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    var ordId = reader.GetInt32(reader.GetOrdinal("id"));
                    var ordUsername = reader.GetString(reader.GetOrdinal("username"));
                    var ordPasswordHash = reader.GetString(reader.GetOrdinal("passwordhash"));
                    var ordRole = (Roles)Enum.Parse(typeof(Roles), reader.GetString(reader.GetOrdinal("role")), true);

                    retUser = new User
                        { Id = ordId, Username = ordUsername, PasswordHash = ordPasswordHash, Role = ordRole };

                    if (!PasswordHelper.VerifyPassword(password, retUser)) return null;
                }
            }
        }

        return retUser;
    }

    public User SelectOne(int id)
    {
        User retUser;

        const string query = $@"SELECT id, username, passwordhash, role FROM {User.CollectionName} WHERE id=@Id";
        using (var conn = new SqlConnection(settings.ConnectionString))
        {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception($"No User with id {id}");

                    var ordId = reader.GetInt32(reader.GetOrdinal("id"));
                    var ordUsername = reader.GetString(reader.GetOrdinal("username"));
                    var ordPasswordHash = reader.GetString(reader.GetOrdinal("passwordhash"));
                    var ordRole = (Roles)Enum.Parse(typeof(Roles), reader.GetString(reader.GetOrdinal("role")), true);

                    retUser = new User
                        { Id = ordId, Username = ordUsername, PasswordHash = ordPasswordHash, Role = ordRole };
                }
            }
        }

        return retUser;
    }

    public void Update(User user, SqlConnection conn, SqlTransaction transaction)
    {
        const string query =
            $"UPDATE {User.CollectionName} SET username=@Username, passwordhash=@PasswordHash, role=@Role WHERE id=@Id";
        using (var cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@Id", user.Id);

            // Execute the command
            cmd.ExecuteNonQuery();
        }
    }

    public User Insert(User user, SqlConnection conn, SqlTransaction transaction)
    {
        int id;
        const string query =
            $"INSERT INTO {User.CollectionName} (name, passwordhash, role) VALUES (@Username, @PasswordHash, @Role)";
        using (var cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", user.Role);

            // Execute the command
            id = cmd.ExecuteNonQuery();
        }

        return new User { Id = id, Username = user.Username, PasswordHash = user.PasswordHash, Role = user.Role };
    }

    public void Delete(int id, SqlConnection conn, SqlTransaction transaction)
    {
        const string query = $"DELETE FROM {User.CollectionName} WHERE id=@Id";
        using (var cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);

            // Execute the command
            cmd.ExecuteNonQuery();
        }
    }
}