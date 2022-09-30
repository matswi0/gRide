using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace gRide.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public byte[]? ProfilePicture { get; set; }
        public RegisterMethod ChosenRegisterMethod { get; set; }
        public ICollection<AppUserFriends> Friends { get; set; }
        public ICollection<Event> EventsHosted { get; set; }
        public List<AppUserEvent> EventsLinked { get; set; }
    }

    public enum RegisterMethod
    {
        Social,
        Email
    }
}
