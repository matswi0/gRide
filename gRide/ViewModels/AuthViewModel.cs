using System.ComponentModel.DataAnnotations;

namespace gRide.ViewModels
{
    public class AuthViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; }
    }
}
