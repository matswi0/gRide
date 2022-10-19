using gRide.Models;

namespace gRide.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Event> EventsHosted { get; set; }
        public IEnumerable<AppUserEvent> EventsLinked { get; set; }
    }
}
