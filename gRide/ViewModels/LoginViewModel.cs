using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace gRide.ViewModels
{
    public class LoginViewModel : AuthViewModel
    {
        [Display(Name = "Remember me")]
        public bool RemeberMe { get; set; }
    }
}
