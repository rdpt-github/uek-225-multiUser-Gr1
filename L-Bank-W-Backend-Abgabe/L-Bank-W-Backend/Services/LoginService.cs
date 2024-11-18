using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace L_Bank_W_Backend.Services;

public class LoginService : ILoginService
{
    private readonly JwtSettings jwtSettings;

    public LoginService(JwtSettings jwtSettings)
    {
        this.jwtSettings = jwtSettings;
        
        if(string.IsNullOrWhiteSpace(this.jwtSettings.IssuerSigninKey))
        {
            throw new ArgumentNullException(nameof(jwtSettings.IssuerSigninKey));
        }
    }
    
    public string CreateJwt(User? user)
    {
        if(user == null || string.IsNullOrWhiteSpace(user.Username))
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this.jwtSettings.IssuerSigninKey!)); // NOTE: USE THE SAME KEY AS USED IN THE PROGRAM.CS OR STARTUP.CS FILE
        var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

        var claims = new[] // NOTE: could also use List<Claim> here
        {
            new Claim(ClaimTypes.Name, user.Username), // NOTE: this will be "User.Identity.Name" value; this could also specify the email address of the user as many sites use for the user name
            new Claim(JwtRegisteredClaimNames.Sub, user.Username), // NOTE: this could be the email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")), // NOTE: this could the unique ID assigned to the user by a database,
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(issuer: "domain.com", audience: "domain.com", claims: claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials); // NOTE: USE THE REAL DOMAIN NAME
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}