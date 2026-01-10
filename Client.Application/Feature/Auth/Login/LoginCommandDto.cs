namespace Client.Application.Feature.Auth.Login
{
    public class LoginCommandDto 
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
