using Microsoft.AspNetCore.Authentication;
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
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords does not match.")]
        public string ReTypePassword { get; set; }
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; } = new List<AuthenticationScheme>();

    }
}
