using System.ComponentModel.DataAnnotations;

namespace gRide.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-type password")]
        [Compare("Password", ErrorMessage = "Passwords does not match")]
        public string ReTypePassword { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
