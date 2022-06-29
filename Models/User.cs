using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public class User : IUserInfo, IUserCredentials
    {
        [Required(ErrorMessage = "{0} is required")]
        [Key]
        [StringLength
            (20, 
            MinimumLength = 4, 
            ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength
            (20,
            MinimumLength = 4,
            ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string Password { get; set; }

        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Invalid email format")]
        [MaxLength(30)]
        public string? Email { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessage = "Invalid age")]
        [Range(0, 200, ErrorMessage = "{0} must be from {1} to {2}")]
        public int? Age { get; set; }

        public User() { }
        public User(User u)
        {
            Username = u.Username;
            Password = u.Password;
            Email = u.Email;
            Age = u.Age;
        }
    }
}
