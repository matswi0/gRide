namespace gRide.Models
{
    public class Localization
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
