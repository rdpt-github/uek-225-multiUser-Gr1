using L_Bank_W_Backend.Core.Models;
using L_Bank.EfCore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L_Bank_W_Backend.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LedgersController : ControllerBase
{
    private readonly ILedgerRepository ledgerRepository;

    public LedgersController(ILedgerRepository ledgerRepository)
    {
        this.ledgerRepository = ledgerRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Administrators,Users")]
    public IEnumerable<Ledger> Get()
    {
        var allLedgers = ledgerRepository.GetAllLedgers();
        return allLedgers;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrators,Users")]
    public Ledger? Get(int id)
    {
        var ledger = ledgerRepository.SelectOne(id);
        return ledger;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrators")]
    public void Put(int id, [FromBody] Ledger ledger)
    {
        ledgerRepository.Update(ledger);
    }

    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public void Post([FromBody] Ledger ledger)
    {
        ledgerRepository.Create(ledger);
    }
}