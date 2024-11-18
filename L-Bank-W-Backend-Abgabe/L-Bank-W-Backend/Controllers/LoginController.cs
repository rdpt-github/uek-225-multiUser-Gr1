using L_Bank_W_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.Services;

namespace L_Bank_W_Backend.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILoginService loginService;

        public LoginController(IUserService userService, ILoginService loginService)
        {
            this.userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            this.loginService = loginService ?? throw new System.ArgumentNullException(nameof(loginService));
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LoginDto login)
        {
            return await Task.Run(() =>
            {
                IActionResult response;

                User? user = this.userService.Authenticate(login.Username, login.Password);
                
                if (user == null)
                {
                    response = Unauthorized();
                }
                else
                {
                    response = Ok(new { token = this.loginService.CreateJwt(user) });
                }
                
                return response;
            });
        }
    }

}
