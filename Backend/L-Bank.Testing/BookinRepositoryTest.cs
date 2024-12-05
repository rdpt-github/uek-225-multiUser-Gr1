using L_Bank_W_Backend.Core.Models;
using L_Bank.EfCore;
using L_Bank.EfCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;

public class BookingRepositoryTest
{
    private readonly Mock<IOptions<DatabaseSettings>> _databaseSettingsMock;

    public BookingRepositoryTest()
    {
        _databaseSettingsMock = new Mock<IOptions<DatabaseSettings>>();
        _databaseSettingsMock.Setup(x => x.Value).Returns(new DatabaseSettings
        {
            ConnectionString =
                "Server=localhost,1433;Database=l_bank_backend;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
        });
    }

    [Fact]
    public void Book_InsufficientBalance_ReturnsFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LBankDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;


        using var context = new LBankDbContext(options);
        context.Database.EnsureCreatedAsync();

        var ledgerRepository = new LedgerRepository(context);
        var bookingRepository = new BookingRepository(context);

        var ledger1 = new Ledger
        {
            Id = 1,
            Name = "abc",
            Balance = 1000000
        };
        var ledger2 = new Ledger
        {
            Id = 2,
            Name = "bde",
            Balance = 1000000
        };
        ledgerRepository.Create(ledger1);
        ledgerRepository.Create(ledger2);

        var ledgers = ledgerRepository.GetAllLedgers().ToList();
        var ledgerId1 = ledgers[0].Id;
        var ledgerId2 = ledgers[0].Id;


        // Arrange
        var amount = 10000001;

        // Act
        var result = bookingRepository.Book(ledgerId1, ledgerId2, amount);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Book_SufficientBalance_ReturnsTrue()
    {
        var options = new DbContextOptionsBuilder<LBankDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;


        using var context = new LBankDbContext(options);
        context.Database.EnsureCreatedAsync();

        var ledgerRepository = new LedgerRepository(context);
        var bookingRepository = new BookingRepository(context);

        var ledger1 = new Ledger
        {
            Id = 1,
            Name = "abc",
            Balance = 100
        };
        var ledger2 = new Ledger
        {
            Id = 2,
            Name = "bde",
            Balance = 100
        };
        ledgerRepository.Create(ledger1);
        ledgerRepository.Create(ledger2);

        // Arrange
        var sourceLedgerId = 1;
        var destinationLedgerId = 2;
        var amount = 100m;

        // Act
        var result = bookingRepository.Book(sourceLedgerId, destinationLedgerId, amount);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Book_TransactionFails_ReturnsFalse()
    {
        var options = new DbContextOptionsBuilder<LBankDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(x =>
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;


        using var context = new LBankDbContext(options);
        context.Database.EnsureCreatedAsync();

        var ledgerRepository = new LedgerRepository(context);
        var bookingRepository = new BookingRepository(context);

        var ledger1 = new Ledger
        {
            Id = 1,
            Name = "abc",
            Balance = 99
        };
        var ledger2 = new Ledger
        {
            Id = 2,
            Name = "bde",
            Balance = 100
        };
        ledgerRepository.Create(ledger1);
        ledgerRepository.Create(ledger2);

        // Act
        var result = bookingRepository.Book(1, 2, 100);

        // Assert
        Assert.False(result);
    }
}