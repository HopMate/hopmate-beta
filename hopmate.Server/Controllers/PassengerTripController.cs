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
    public class PassengerTripController : ControllerBase
    {
        private readonly PassengerTripService _passengerTripService;
        private readonly ApplicationDbContext _context;

        public PassengerTripController(PassengerTripService passengerTripService, ApplicationDbContext context)
        {
            _passengerTripService = passengerTripService;
            _context = context;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET: api/PassengerTrip/my-requests
        [HttpGet("my-requests")]
        public async Task<ActionResult<List<PassengerTrip>>> GetMyRequests()
        {
            try
            {
                var userId = GetUserId();
                var passenger = await _context.Passengers.FindAsync(userId);
                if (passenger == null)
                    return BadRequest("User is not registered as a passenger");

                var requests = await _passengerTripService.GetRequestsByPassengerIdAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/PassengerTrip/my-driver-trips
        [HttpGet("my-driver-trips")]
        public async Task<IActionResult> GetMyDriverTrips()
        {
            try
            {
                var userId = GetUserId();
                var trips = await _context.Trips
                    .Include(t => t.Vehicle)
                    .Include(t => t.Driver)
                    .Where(t => t.IdDriver == userId)
                    .Select(t => new
                    {
                        t.Id,
                        t.DtDeparture,
                        t.DtArrival,
                        t.AvailableSeats,
                        Vehicle = new
                        {
                            t.Vehicle.Brand,
                            t.Vehicle.Model
                        },
                        Driver = new
                        {
                            t.Driver.Name
                        }
                    })
                    .ToListAsync();

                return Ok(trips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/PassengerTrip/my-passenger-trips
        [HttpGet("my-passenger-trips")]
        public async Task<IActionResult> GetMyPassengerTrips()
        {
            try
            {
                var userId = GetUserId();
                var trips = await _context.PassengerTrips
                    .Include(pt => pt.Trip)
                        .ThenInclude(t => t.Driver)
                    .Include(pt => pt.RequestStatus)
                    .Where(pt => pt.IdPassenger == userId)
                    .Select(pt => new
                    {
                        pt.Trip.Id,
                        pt.Trip.DtDeparture,
                        pt.Trip.DtArrival,
                        pt.Trip.AvailableSeats,
                        Driver = new
                        {
                            pt.Trip.Driver.Name
                        },
                        RequestStatus = new
                        {
                            pt.RequestStatus.Status
                        }
                    })
                    .ToListAsync();

                return Ok(trips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/PassengerTrip
        [HttpPost]
        public async Task<ActionResult<PassengerTrip>> CreateRequest([FromBody] PassengerTrip request)
        {
            try
            {
                var userId = GetUserId();
                var passenger = await _context.Passengers.FindAsync(userId);
                if (passenger == null)
                    return BadRequest("User is not registered as a passenger");

                var trip = await _context.Trips
                    .Include(t => t.Driver)
                    .FirstOrDefaultAsync(t => t.Id == request.IdTrip);

                if (trip == null)
                    return NotFound("Trip not found");

                if (trip.Driver.IdUser == userId)
                    return BadRequest("You cannot request to join your own trip");

                request.IdPassenger = userId;
                request.DateRequest = DateTime.UtcNow;

                var createdRequest = await _passengerTripService.CreateRequestAsync(request);
                return CreatedAtAction(nameof(GetRequest), new { id = createdRequest.Id }, createdRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PassengerTrip/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerTrip>> GetRequest(Guid id)
        {
            try
            {
                var request = await _passengerTripService.GetRequestByIdAsync(id);
                if (request == null)
                    return NotFound();

                var userId = GetUserId();
                if (request.IdPassenger != userId && request.Trip?.Driver?.IdUser != userId)
                    return Forbid();

                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/PassengerTrip/cancel/{id}
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelRequest(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var request = await _context.PassengerTrips
                    .Include(pt => pt.Trip)
                    .FirstOrDefaultAsync(pt => pt.Id == id);

                if (request == null)
                    return NotFound("Request not found");

                if (request.IdPassenger != userId)
                    return Forbid();

                await _passengerTripService.UpdateRequestStatusAsync(id, 5); // 5 = Canceled
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}