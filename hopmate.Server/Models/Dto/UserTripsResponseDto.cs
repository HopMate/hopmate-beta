namespace hopmate.Server.Models.Dto
{
    public class UserTripsResponseDto
    {
        public List<TripSummaryDto> DriverTrips { get; set; } = new();
        public List<TripSummaryDto> PassengerTrips { get; set; } = new();
    }
}
