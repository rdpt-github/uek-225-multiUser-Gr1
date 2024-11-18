using System.Data.SqlClient;
using L_Bank_W_Backend.Core;
using L_Bank_W_Backend.Core.Helper;
using L_Bank_W_Backend.Core.Models;

namespace L_Bank_W_Backend.DbAccess
{
    public interface IDatabaseSeeder
    {
        void Initialize();
        void Seed();
    }
}