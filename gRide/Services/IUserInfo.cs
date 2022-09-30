using gRide.Models;
using gRide.ViewModels;

namespace gRide.Services
{
    public interface IUserInfo
    {
        public DisplayUserViewModel NormalizeUser(Guid id, string userName, byte[] pciture);
        public ICollection<DisplayUserViewModel> NormalizeFriendList(AppUser user);
    }
}