﻿using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.DbAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace L_Bank_W_Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MoneyPrinterController(IMoneyPrinterRepository moneyPrinterRepository) : ControllerBase
{
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post(int id)
    {
        return await Task.Run(() =>
        {
            IActionResult response;
                
            if(moneyPrinterRepository.Add100(id))
            {
                response = Ok();
            } else
            {
                response = Conflict();
            }
            return response;
        });
    }
}