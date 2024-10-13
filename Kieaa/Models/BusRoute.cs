namespace Kieaa.Models
{
    public class BusRoute
    {
        public int Id { get; set; }
        public string StartName { get; set; } = string.Empty;
        public string EndName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public List<string> Cites { get; set; } = new List<string>();

        public string UserId { get; set; }
        public User CreatedBy { get; set; }

        public List<RouteCoordinate>? RouteCoordinates { get; set; }

    }
}
