using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace gRide.ViewModels
{
    public class RegisterViewModel : AuthViewModel
    {
        [Required]
        [MaxLength(20)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-type password")]
        [Compare("Password", ErrorMessage = "Passwords does not match.")]
        public string ReTypePassword { get; set; }
    }
}
