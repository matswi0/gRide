using gRide.Models;
using Microsoft.EntityFrameworkCore;

namespace gRide.Data
{
    public class gRideDbContext : DbContext
    {
        public gRideDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
