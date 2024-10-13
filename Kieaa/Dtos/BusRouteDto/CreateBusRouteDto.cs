namespace Kieaa.Dtos.BusRouteDto
{
    public class CreateBusRouteDto
    {
        public string Name { get; set; }
        public string StartName { get; set; } = string.Empty;
        public string EndName { get; set; } = string.Empty;
        public List<string> Cites { get; set; } = new List<string>();
        public List<CoordinateDto> Coordinates { get; set; }
    }
}
