using gRide.Data;
using gRide.Models;
using gRide.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace gRide.Services
{
    public class UserInfo : IUserInfo
    {
        private readonly gRideDbContext _dbContext;

        public UserInfo(gRideDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ICollection<DisplayUserViewModel> NormalizeFriendList(AppUser user)
        {
            var friendList = _dbContext.AppUsersFriends
                .Include(uf => uf.FriendUser)
                .Where(uf => uf.UserId == user.Id && uf.IsConfirmed == true)
                .Select(uf => new { uf.FriendId, uf.FriendUser.ProfilePicture, uf.FriendUser.UserName }).OrderBy(x => x.UserName);

            ICollection<DisplayUserViewModel> normalizedFriendList = new List<DisplayUserViewModel>();

            foreach (var friend in friendList)
            {
                normalizedFriendList.Add(NormalizeUser(friend.FriendId, friend.UserName, friend.ProfilePicture));
            }
            return normalizedFriendList;
        }

        public DisplayUserViewModel NormalizeUser(Guid id, string userName, byte[] pciture)
        {
            string byteArrayToBase64 = Convert.ToBase64String(pciture);
            string profilePicture = $"data:image/png;base64,{byteArrayToBase64}";
            var normalizedUser = new DisplayUserViewModel
            {
                UserId = id.ToString(),
                UserName = userName,
                ProfilePicture = profilePicture
            };
            return normalizedUser;
        }
    }
}
