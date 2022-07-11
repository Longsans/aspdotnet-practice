namespace Common.Models
{
    public class LoginResponse
    {
        public User? User { get; set; }
        public string? Jwt { get; set; }
        public Dictionary<string, string?>? Errors { get; set; }
    }
}