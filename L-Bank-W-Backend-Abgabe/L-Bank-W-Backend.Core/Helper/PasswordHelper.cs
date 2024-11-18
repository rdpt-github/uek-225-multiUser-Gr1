namespace L_Bank_W_Backend.Core.Helper;

public static class PasswordHelper
{
    public static string HashAndSaltPassword(string clearTextPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(clearTextPassword);
    }
}