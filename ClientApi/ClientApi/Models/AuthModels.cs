namespace ClientApi.Models
{
    // Pentru login
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Pentru înregistrare
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Răspuns după login/register
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }
}