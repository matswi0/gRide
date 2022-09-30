using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace gRide.Models
{
    public class AppUserEvent
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public AppUser User { get; set; }
        public Event Event { get; set; }
        public UserStatus UserStatus { get; set; }
    }
    
    public enum UserStatus
    {
        Invited,
        Going
    }
}
