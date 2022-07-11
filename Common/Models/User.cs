using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class User : IUserInfo, IUserCredentials
    {
        [Key]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string Username { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string Password { get; set; }


        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string? FirstName { get; set; }


        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [MaxLength(20)]
        public string? LastName { get; set; }

        
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Invalid email format")]
        [MaxLength(30)]
        public string? Email { get; set; }

        
        [RegularExpression("([0-9]+)", ErrorMessage = "Invalid age")]
        [Range(0, 200, ErrorMessage = "{0} must be from {1} to {2}")]
        public int? Age { get; set; }

        public Contact? Contact { get; set; }

        public User() { }
        public User(User u)
        {
            Username = u.Username;
            Password = u.Password;
            FirstName = u.FirstName;
            LastName = u.LastName;
            Email = u.Email;
            Age = u.Age;
        }
    }
}
