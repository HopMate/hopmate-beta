using Microsoft.AspNetCore.Mvc;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DriverTripController : ControllerBase
    {
        private readonly PassengerTripService _passengerTripService;
        private readonly ApplicationDbContext _context;

        public DriverTripController(PassengerTripService passengerTripService, ApplicationDbContext context)
        {
            _passengerTripService = passengerTripService;
            _context = context;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET: api/DriverTrip/requests/{tripId}
        [HttpGet("requests/{tripId}")]
        public async Task<ActionResult<List<PassengerTrip>>> GetTripRequests(Guid tripId)
        {
            var userId = GetUserId();

            // Verificar se o usuário é o motorista desta viagem
            var trip = await _context.Trips
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                return NotFound("Trip not found");

            if (trip.Driver.IdUser != userId)
                return Forbid();

            var requests = await _passengerTripService.GetRequestsByTripIdAsync(tripId);
            return Ok(requests);
        }

        // PUT: api/DriverTrip/accept/{requestId}
        [HttpPut("accept/{requestId}")]
        public async Task<ActionResult> AcceptRequest(Guid requestId)
        {
            var userId = GetUserId();
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .ThenInclude(t => t.Driver)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            if (request.Trip.Driver.IdUser != userId)
                return Forbid();

            try
            {
                await _passengerTripService.UpdateRequestStatusAsync(requestId, 2); // Accepted
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/DriverTrip/reject/{requestId}
        [HttpPut("reject/{requestId}")]
        public async Task<ActionResult> RejectRequest(Guid requestId, [FromBody] string reason)
        {
            var userId = GetUserId();
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .ThenInclude(t => t.Driver)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            if (request.Trip.Driver.IdUser != userId)
                return Forbid();

            if (string.IsNullOrWhiteSpace(reason))
                return BadRequest("Reason is required for rejection");

            try
            {
                await _passengerTripService.UpdateRequestStatusAsync(requestId, 3, reason); // Rejected
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/DriverTrip/waiting-list/{requestId}
        [HttpPut("waiting-list/{requestId}")]
        public async Task<ActionResult> MoveToWaitingList(Guid requestId)
        {
            var userId = GetUserId();
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .ThenInclude(t => t.Driver)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            if (request.Trip.Driver.IdUser != userId)
                return Forbid();

            try
            {
                await _passengerTripService.UpdateRequestStatusAsync(requestId, 4); // WaitingList
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}