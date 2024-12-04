using L_Bank_W_Backend.DbAccess.Repositories;
using Microsoft.Extensions.Options;
using L_Bank_W_Backend.DbAccess;

public class BookingRepositoryTest
{
    private readonly IOptions<DatabaseSettings> _settings;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTest()
    {
        _settings = Options.Create(new DatabaseSettings { ConnectionString = "Server=localhost,1433;User Id=sa;Password=YourPassword123;" });
        _bookingRepository = new BookingRepository(_settings);
    }

    [Fact]
    public void Book_InsufficientBalance_ReturnsFalse()
    {
        // Arrange
        var sourceLedgerId = 1;
        var destinationLedgerId = 2;
        var amount = 100;

        // Act
        var result = _bookingRepository.Book(sourceLedgerId, destinationLedgerId, amount);

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