using Common.Models;

namespace Practice.ViewModels
{
    public class BaseUserViewModel : IPageErrorAndSuccessMessage
    {
        public User? User { get; set; }
        public string? PageError { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
