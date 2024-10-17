namespace Kieaa.Models
{
    public class RouteCoordinate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int BusRouteId { get; set; }
        public BusRoute? BusRoute { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Add an Order field to determine the sequence of coordinates
        public int Order { get; set; }

        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
    }
}
