using gRide.Data;
using gRide.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace gRide.DAL
{
    public static class ModelBuilderExtensions
    {
        private static IWebHostEnvironment _env;
        private const string defaultProfilePicture = "profile_picture.png";
        public static void Seed(this ModelBuilder modelBuilder, IWebHostEnvironment env)
        {
            _env = env;

            //Users
            AppUser user1 = CreateUser("6182c48f-4e41-4609-808e-f6df95e5d85f", "Mateusz", "user1@users.net");
            AppUser user2 = CreateUser("d4308670-acc9-439c-ae79-74e125d0dba9", "Andrzej", "user2@users.net");
            AppUser user3 = CreateUser("4c32b38a-4aec-4531-bc02-0cfda8e160a5", "Karol", "user3@users.net");
            AppUser user4 = CreateUser("e89503ad-21ba-4556-ac7c-e11f871f2113", "Jan", "user4@users.net");
            AppUser user5 = CreateUser("208d0cec-4783-4e15-aebe-798412bde75d", "Piotr", "user5@users.net");
            modelBuilder.Entity<AppUser>().HasData(
                user1, user2, user3, 
                user4, user5);

            //Friends
            //user1: user2, user3, user4 without user5
            AppUserFriends appUserFriends1 = AddFriend("6182c48f-4e41-4609-808e-f6df95e5d85f", "d4308670-acc9-439c-ae79-74e125d0dba9");
            AppUserFriends appUserFriends2 = AddFriend("6182c48f-4e41-4609-808e-f6df95e5d85f", "4c32b38a-4aec-4531-bc02-0cfda8e160a5");
            AppUserFriends appUserFriends3 = AddFriend("6182c48f-4e41-4609-808e-f6df95e5d85f", "e89503ad-21ba-4556-ac7c-e11f871f2113");

            AppUserFriends appUserFriends11 = AddFriend("d4308670-acc9-439c-ae79-74e125d0dba9", "6182c48f-4e41-4609-808e-f6df95e5d85f");
            AppUserFriends appUserFriends21 = AddFriend("4c32b38a-4aec-4531-bc02-0cfda8e160a5", "6182c48f-4e41-4609-808e-f6df95e5d85f");
            AppUserFriends appUserFriends31 = AddFriend("e89503ad-21ba-4556-ac7c-e11f871f2113", "6182c48f-4e41-4609-808e-f6df95e5d85f");

            //user4: user2, user3  without user1 and user5
            AppUserFriends appUserFriends4 = AddFriend("e89503ad-21ba-4556-ac7c-e11f871f2113", "d4308670-acc9-439c-ae79-74e125d0dba9");
            AppUserFriends appUserFriends5 = AddFriend("e89503ad-21ba-4556-ac7c-e11f871f2113", "4c32b38a-4aec-4531-bc02-0cfda8e160a5");

            AppUserFriends appUserFriends41 = AddFriend("d4308670-acc9-439c-ae79-74e125d0dba9", "e89503ad-21ba-4556-ac7c-e11f871f2113");
            AppUserFriends appUserFriends51 = AddFriend("4c32b38a-4aec-4531-bc02-0cfda8e160a5", "e89503ad-21ba-4556-ac7c-e11f871f2113");
            modelBuilder.Entity<AppUserFriends>().HasData(
                appUserFriends1, appUserFriends2, appUserFriends3, 
                appUserFriends4, appUserFriends5, appUserFriends11, 
                appUserFriends21, appUserFriends31, appUserFriends41, 
                appUserFriends51);
        }

        private static AppUser CreateUser(string Id, string UserName, string Email, bool EmailConfirmed = true, string Password = "Test123%",
            RegisterMethod ChosenRegisterMehtod = RegisterMethod.Email)
        {
            var hasher = new PasswordHasher<AppUser>();
            byte[] profilePicture;
            try
            {
                string path = Path.Combine(_env.WebRootPath, "img", defaultProfilePicture);
                profilePicture = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            AppUser user = new()
            {
                Id = new Guid(Id),
                UserName = UserName,
                Email = Email,
                NormalizedUserName = UserName.ToUpper(),
                NormalizedEmail = Email.ToUpper(),
                EmailConfirmed = EmailConfirmed,
                PasswordHash = hasher.HashPassword(null, Password),
                SecurityStamp = string.Empty,
                ProfilePicture = profilePicture,
                ChosenRegisterMethod = ChosenRegisterMehtod
            };
            return user;
        }

        private static AppUserFriends AddFriend(string toId, string friendId, bool isConfirmed = true, bool isRejected = false)
        {
            AppUserFriends appUserFriends = new()
            {
                UserId = new Guid(toId),
                FriendId = new Guid(friendId),
                IsConfirmed = isConfirmed,
                IsRejected = isRejected,
            };
            return appUserFriends;
        }
    }
}
