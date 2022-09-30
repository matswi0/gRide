using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace gRide.ViewModels
{
    public class EditProfileViewModel
    {
        [Required, DisplayName("Profile picture")]
        public IFormFile ProfilePicture { get; set; }
    }
}
