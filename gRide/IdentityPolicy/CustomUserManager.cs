﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Resources;

namespace gRide.IdentityPolicy
{
    public class CustomUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        public CustomUserManager(IUserStore<TUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<TUser> passwordHasher, 
            IEnumerable<ICustomUserValidator<TUser>> userValidators, 
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, ILogger<UserManager<TUser>> logger) : base(store, 
                optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
