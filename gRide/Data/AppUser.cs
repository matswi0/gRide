using Microsoft.AspNetCore.Identity;

namespace gRide.Data
{
    public class AppUser : IdentityUser
    {
        public byte[]? ProfilePicture { get; set; }
        public RegisterMethod ChosenRegisterMethod { get; set; }
    }

    public enum RegisterMethod
    {
        Social,
        Email
    }
}
