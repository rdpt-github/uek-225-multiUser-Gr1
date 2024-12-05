using L_Bank_W_Backend.Dto;
using L_Bank.EfCore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L_Bank_W_Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GambleController : ControllerBase
{
    private readonly IGambleRepository _gambleRepository;

    public GambleController(IGambleRepository gambleRepository)
    {
        _gambleRepository = gambleRepository;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Put(int id, [FromBody] int[] numbers)
    {
        var result = CalculateWin(numbers);

        return await Task.Run(() =>
        {
            IActionResult response;

            if (_gambleRepository.Gamble(id, result.WinAmount))
                response = Ok(result);
            else
                response = Conflict();
            return response;
        });
    }

    private GambleResultDto CalculateWin(int[] numbers)
    {
        var random = new Random();
        var counter = 0;
        var win = 0;
        for (var i = 0; i < numbers.Length; i++)
        {
            var randomNumber = random.Next(1, 7);
            if (numbers[i] == randomNumber) counter++;
        }

        switch (counter)
        {
            case 0:
                win = -10;
                break;
            case 1:
                win = -10;
                break;
            case 2:
                win = 20;
                break;
            case 3:
                win = 1000;
                break;
        }

        return new GambleResultDto
        {
            CorrectNumbers = counter,
            WinAmount = win
        };
    }
}