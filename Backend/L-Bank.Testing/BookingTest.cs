using L_Bank_W_Backend.Controllers;
using L_Bank_W_Backend.Core.Models;
using L_Bank.EfCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestProject1;

public class BookingTest
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly BookingsController _bookingsController;

    public BookingTest()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingsController = new BookingsController(_bookingRepositoryMock.Object);
    }

    [Fact]
    public async Task Post_BookingSuccessful_ReturnsOk()
    {
        // Arrange
        var booking = new Booking { SourceId = 1, DestinationId = 2, Amount = 100 };
        _bookingRepositoryMock.Setup(repo => repo.Book(booking.SourceId, booking.DestinationId, booking.Amount))
            .Returns(true);

        // Act
        var result = await _bookingsController.Post(booking);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Post_BookingConflict_ReturnsConflict()
    {
        // Arrange
        var booking = new Booking { SourceId = 1, DestinationId = 2, Amount = 100 };
        _bookingRepositoryMock.Setup(repo => repo.Book(booking.SourceId, booking.DestinationId, booking.Amount))
            .Returns(false);

        // Act
        var result = await _bookingsController.Post(booking);

        // Assert
        var conflictResult = Assert.IsType<ConflictResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
    }
}