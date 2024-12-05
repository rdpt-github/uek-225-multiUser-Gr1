using L_Bank_W_Backend.Core.Models;
using Microsoft.Data.SqlClient;

namespace L_Bank.EfCore.Repositories;

public interface IUserRepository
{
    User? Authenticate(string? username, string? password);
    User SelectOne(int id);
    void Update(User user, SqlConnection conn, SqlTransaction transaction);
    User Insert(User user, SqlConnection conn, SqlTransaction transaction);
    void Delete(int id, SqlConnection conn, SqlTransaction transaction);
}