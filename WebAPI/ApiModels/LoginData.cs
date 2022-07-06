using Common.Models;

namespace WebAPI.ApiModels
{
    public class LoginData : IUserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberUser { get; set; }
    }
}
