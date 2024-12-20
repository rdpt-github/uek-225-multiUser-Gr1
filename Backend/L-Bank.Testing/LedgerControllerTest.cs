﻿using L_Bank_W_Backend.Controllers;
using L_Bank_W_Backend.Core.Models;
using L_Bank.EfCore.Repositories;
using Moq;

public class LedgersControllerTest
{
    private readonly LedgersController ledgersController;
    private readonly Mock<ILedgerRepository> mockLedgerRepository;

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