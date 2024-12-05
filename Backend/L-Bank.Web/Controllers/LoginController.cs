using L_Bank_W_Backend.Dto;
using L_Bank_W_Backend.Services;
using L_Bank.EfCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace L_Bank_W_Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService loginService;
    private readonly IUserRepository userRepository;

    public LoginController(IUserRepository userRepository, ILoginService loginService)
    {
        this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LoginDto login)
    {
        return await Task.Run(() =>
        {
            IActionResult response;

            var user = userRepository.Authenticate(login.Username, login.Password);

            if (user == null)
                response = Unauthorized();
            else
                response = Ok(new { token = loginService.CreateJwt(user) });

            return response;
        });
    }
}