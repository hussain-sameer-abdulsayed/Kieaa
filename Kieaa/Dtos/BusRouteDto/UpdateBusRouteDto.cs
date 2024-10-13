using Kieaa.Models;

namespace Kieaa.Dtos.BusRouteDto
{
    public class UpdateBusRouteDto
    {
        public string? StartName { get; set; } = string.Empty;
        public string? EndName { get; set; } = string.Empty;

        public List<string>? Cites { get; set; } = new List<string>();

        public List<CoordinateDto>? RouteCoordinates { get; set; } = new List<CoordinateDto>();
    }
}
