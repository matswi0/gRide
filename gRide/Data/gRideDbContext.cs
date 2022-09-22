using gRide.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gRide.Data
{
    public class gRideDbContext : IdentityDbContext<AppUser>
    {
        public gRideDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
