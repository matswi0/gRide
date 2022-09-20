using Microsoft.AspNetCore.Identity;

namespace gRide.IdentityPolicy
{
    public interface ICustomUserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
    }
}