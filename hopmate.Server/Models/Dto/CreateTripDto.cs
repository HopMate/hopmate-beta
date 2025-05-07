using System.ComponentModel.DataAnnotations;

namespace hopmate.Server.Models.Dto
{
    public class CreateTripDto
    {
        [Required]
        public Guid DriverId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public LocationDto StartLocation { get; set; }

        [Required]
        public LocationDto EndLocation { get; set; }
    }
}
