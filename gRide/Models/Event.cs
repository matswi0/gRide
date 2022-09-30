using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace gRide.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public AppUser Host { get; set; }
        public Localization Localization { get; set; }
        public List<AppUserEvent> UsersLinked { get; set; }
    }
}
