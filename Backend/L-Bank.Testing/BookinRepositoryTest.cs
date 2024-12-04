using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.DbAccess;
using L_Bank_W_Backend.DbAccess.Repositories;
using Microsoft.Extensions.Options;

public class BookingRepositoryTest
{
    private readonly BookingRepository _bookingRepository;
    private readonly LedgerRepository _ledgerRepository;
    private readonly IOptions<DatabaseSettings> _settings;

    public BookingRepositoryTest()
    {
        _settings = Options.Create(new DatabaseSettings
        {
            ConnectionString = "Server=localhost,1433;Database=l_bank_backend;User Id=sa;Password=YourPassword123;"
        });
        _bookingRepository = new BookingRepository(_settings);
        _ledgerRepository = new LedgerRepository(_settings);
    }

    [Fact]
    public void Book_InsufficientBalance_ReturnsFalse()
    {
        var ledger1 = new Ledger
        {
            Id = 1,
            Name = "abc",
            Balance = 1000000
        };
        var ledger2 = new Ledger
        {
            Id = 1,
            Name = "bde",
            Balance = 1000000
        };
        _ledgerRepository.Create(ledger1);
        _ledgerRepository.Create(ledger2);

        var ledgers = _ledgerRepository.GetAllLedgers().ToList();
        var ledgerId1 = ledgers[^1].Id;
        var ledgerId2 = ledgers[^2].Id;


        // Arrange
        var amount = 10000001;

        // Act
        var result = _bookingRepository.Book(ledgerId1, ledgerId2, amount);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Book_SufficientBalance_ReturnsTrue()
    {
        // Arrange
        var sourceLedgerId = 1;
        var destinationLedgerId = 2;
        var amount = 100m;

        // Act
        var result = _bookingRepository.Book(sourceLedgerId, destinationLedgerId, amount);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Book_TransactionFails_ReturnsFalse()
    {
        // Arrange
        var sourceLedgerId = 1;
        var destinationLedgerId = 2;
        var amount = 100m;

        // Simulate a failure by providing invalid data or causing an exception
        // For example, you can use an invalid ledgerId or amount

        // Act
        var result = _bookingRepository.Book(sourceLedgerId, destinationLedgerId, amount);

        // Assert
        Assert.False(result);
    }
}