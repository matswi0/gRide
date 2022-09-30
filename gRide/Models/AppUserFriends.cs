namespace gRide.Models
{
    public class AppUserFriends
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public AppUser User { get; set; }
        public AppUser FriendUser { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRejected { get; set; }
    }
}
