using Microsoft.AspNetCore.Identity;

namespace gRide.Data
{
    public class AppUser : IdentityUser
    {
        public byte[] ProfilePicture { get; set; }
    }
}
