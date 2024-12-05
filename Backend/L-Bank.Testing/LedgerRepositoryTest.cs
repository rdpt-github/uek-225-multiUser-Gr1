using L_Bank_W_Backend.DbAccess.Repositories;
using L_Bank_W_Backend.Core.Models;
using Moq;
using System.Data.SqlClient;
using L_Bank_W_Backend.DbAccess;
using Microsoft.Extensions.Options;
using Xunit;

public class LedgerRepositoryTests
{
    private readonly LedgerRepository _ledgerRepository;
    private readonly IOptions<DatabaseSettings> _settings;

    public LedgerRepositoryTests()
    {
        _settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = "Server=localhost,1433;Database=l_bank_backend;User Id=sa;Password=YourPassword123;"
        });
        _ledgerRepository = new LedgerRepository(_settings);
    }

    [Fact]
    public void Create_ShouldAddNewLedger()
    {
        // Arrange
        var ledger = new Ledger { Name = "Test Ledger", Balance = 100 };

        // Act
        _ledgerRepository.Create(ledger);

        // Assert
        var loadedLedger = _ledgerRepository.FindLedgerByName(ledger.Name);
        Assert.NotNull(loadedLedger);
        _ledgerRepository.Delete(loadedLedger!.Id);
    }
    
    [Fact]
    public void Create_ShouldThrowSqlException()
    {
        // Arrange
        var ledger = new Ledger { Name = null, Balance = 0 };

        // Act & Assert
        Assert.Throws<SqlException>(() => _ledgerRepository.Create(ledger));
    }
}