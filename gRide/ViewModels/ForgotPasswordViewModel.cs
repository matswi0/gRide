using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace gRide.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
