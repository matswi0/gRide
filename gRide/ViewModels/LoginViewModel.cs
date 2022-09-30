using System.ComponentModel.DataAnnotations;

namespace gRide.ViewModels
{
    public class LoginViewModel : AuthViewModel
    {
        [Display(Name = "Remember me")]
        public bool RemeberMe { get; set; }
    }
}
