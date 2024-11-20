using System.Data.SqlClient;
using L_Bank_W_Backend.Core.Models;

namespace L_Bank_W_Backend.DbAccess;

public interface ILedgerRepository
{
    IEnumerable<Ledger> GetAllLedgers();
    Ledger? SelectOne(int id);
    Ledger? SelectOne(int id, SqlConnection conn, SqlTransaction? transaction);
    void Update(Ledger ledger, SqlConnection conn, SqlTransaction transaction);
    void Update(Ledger ledger);

}