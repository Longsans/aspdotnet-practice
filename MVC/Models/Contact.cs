using System.ComponentModel.DataAnnotations;

namespace Practice.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(11)]
        [RegularExpression(@"^0[1-9]+$")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "{0} number must be from {2} to {1} digits")]
        public string Phone { get; set; }

        [Required]
        [MaxLength(75)]
        [StringLength(75, MinimumLength = 1, ErrorMessage = "{0} must be from {2} to {1} digits")]
        public string Address { get; set; }

        public User User { get; set; }
        public string UserUsername { get; set; }
    }
}
