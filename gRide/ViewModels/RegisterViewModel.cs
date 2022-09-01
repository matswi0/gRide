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
    }
}
