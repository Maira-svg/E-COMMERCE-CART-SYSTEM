namespace DarazUltimateMVC.Services
{
    public class PasswordService
    {
        // Password ko hash mein convert karega - Store ke liye
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        // Login time par password verify karega
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
