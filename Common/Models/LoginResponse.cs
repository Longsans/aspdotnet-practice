namespace Common.Models
{
    public class LoginResponse
    {
        public object? User { get; set; }
        public string? AccessToken { get; set; }
        public Dictionary<string, string?>? Errors { get; set; }
    }
}