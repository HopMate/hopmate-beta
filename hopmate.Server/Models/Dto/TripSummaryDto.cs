namespace hopmate.Server.Models.Dto
{
    public class TripSummaryDto
    {
        public Guid TripId { get; set; }
        public DateTimeOffset DepartureTime { get; set; }
        public DateTimeOffset ArrivalTime { get; set; }
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        
    }
}
