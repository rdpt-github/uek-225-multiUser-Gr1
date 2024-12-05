using L_Bank_W_Backend.Controllers;
using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.DbAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class LedgersControllerTest
{
    private readonly Mock<ILedgerRepository> mockLedgerRepository;
    private readonly LedgersController ledgersController;

    public LedgersControllerTest()
    {
        mockLedgerRepository = new Mock<ILedgerRepository>();
        ledgersController = new LedgersController(mockLedgerRepository.Object);
    }

    [Fact]
    public void Post_ShouldCallCreateMethod()
    {
        // Arrange
        var ledger = new Ledger { Name = "Test Ledger", Balance = 100 };

        // Act
        ledgersController.Post(ledger);

        // Assert
        mockLedgerRepository.Verify(repo => repo.Create(It.IsAny<Ledger>()), Times.Once);
    }
}