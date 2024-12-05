using L_Bank_W_Backend.Core.Models;
using L_Bank.EfCore;
using L_Bank.EfCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace TestProject1;

public class LedgerRepositoryTest
{
    private readonly Mock<IOptions<DatabaseSettings>> _databaseSettingsMock;
    private readonly ITestOutputHelper _testOutputHelper;

    public LedgerRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _databaseSettingsMock = new Mock<IOptions<DatabaseSettings>>();
        _databaseSettingsMock.Setup(x => x.Value).Returns(new DatabaseSettings
        {
            ConnectionString =
                "Server=localhost,1433;Database=l_bank_backend;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
        });
    }

    [Fact]
    public void Create_ShouldAddNewLedger()
    {
        var options = new DbContextOptionsBuilder<LBankDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;


        using var context = new LBankDbContext(options);
        context.Database.EnsureCreatedAsync();

        var ledgerRepository = new LedgerRepository(context);
        // Arrange
        var ledger = new Ledger { Name = "Test Ledger", Balance = 100 };

        // Act
        ledgerRepository.Create(ledger);

        // Assert
        var loadedLedger = context.Ledgers.FirstOrDefault(l => ledger.Id == l.Id);
        Assert.NotNull(loadedLedger);
    }

    [Fact]
    public void Create_ShouldThrowSqlException()
    {
        var options = new DbContextOptionsBuilder<LBankDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;


        using var context = new LBankDbContext(options);
        context.Database.EnsureCreatedAsync();

        var ledgerRepository = new LedgerRepository(context);

        // Arrange
        var ledger = new Ledger { Name = null, Balance = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ledgerRepository.Create(ledger));
    }
}