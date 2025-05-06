using hopmate.Server.Data;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hopmate.Server.Services
{
    public class TripParticipationService
    {
        private readonly ApplicationDbContext _context;

        public TripParticipationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a trip has available seats
        /// </summary>
        /// <param name="tripId">The ID of the trip to check</param>
        /// <returns>True if the trip has available seats, false otherwise</returns>
        public async Task<bool> HasAvailableSeatsAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.PassengerTrips.Where(pt => pt.IdRequestStatus == 2)) // Only count accepted requests
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            return trip.AvailableSeats > trip.PassengerTrips.Count;
        }

        /// <summary>
        /// Creates a participation request for a passenger to join a trip
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="passengerId">The ID of the passenger</param>
        /// <param name="locationId">The pickup location ID</param>
        /// <returns>The created PassengerTrip entity</returns>
        public async Task<PassengerTrip> CreateParticipationRequestAsync(Guid tripId, Guid passengerId, Guid locationId)
        {
            // Verify the trip exists
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            // Verify the passenger exists
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.IdUser == passengerId);

            if (passenger == null)
                throw new ArgumentException("Passenger not found", nameof(passengerId));

            // Verify the location exists
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId);

            if (location == null)
                throw new ArgumentException("Location not found", nameof(locationId));

            // Check if the passenger already has a request for this trip
            var existingRequest = await _context.PassengerTrips
                .FirstOrDefaultAsync(pt => pt.IdPassenger == passengerId && pt.IdTrip == tripId);

            if (existingRequest != null)
                throw new InvalidOperationException("Passenger already has a request for this trip");

            var passengerTrip = new PassengerTrip
            {
                Id = Guid.NewGuid(),
                IdPassenger = passengerId,
                IdTrip = tripId,
                IdLocation = locationId,
                IdRequestStatus = 1, // Pending status
                DateRequest = DateTime.UtcNow
            };

            _context.PassengerTrips.Add(passengerTrip);
            await _context.SaveChangesAsync();

            return passengerTrip;
        }

        /// <summary>
        /// Adds a passenger to the waiting list for a trip
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="passengerId">The ID of the passenger</param>
        /// <param name="locationId">The pickup location ID</param>
        /// <returns>The created PassengerTrip entity with WaitingList status</returns>
        public async Task<PassengerTrip> AddToWaitingListAsync(Guid tripId, Guid passengerId, Guid locationId)
        {
            // Verify the trip exists
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            // Verify the passenger exists
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.IdUser == passengerId);

            if (passenger == null)
                throw new ArgumentException("Passenger not found", nameof(passengerId));

            // Verify the location exists
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId);

            if (location == null)
                throw new ArgumentException("Location not found", nameof(locationId));

            // Check if the passenger already has a request for this trip
            var existingRequest = await _context.PassengerTrips
                .FirstOrDefaultAsync(pt => pt.IdPassenger == passengerId && pt.IdTrip == tripId);

            if (existingRequest != null)
                throw new InvalidOperationException("Passenger already has a request for this trip");

            var passengerTrip = new PassengerTrip
            {
                Id = Guid.NewGuid(),
                IdPassenger = passengerId,
                IdTrip = tripId,
                IdLocation = locationId,
                IdRequestStatus = 4, // WaitingList status
                DateRequest = DateTime.UtcNow
            };

            _context.PassengerTrips.Add(passengerTrip);
            await _context.SaveChangesAsync();

            return passengerTrip;
        }

        /// <summary>
        /// Gets all participation requests for a trip
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetRequestsByTripAsync(Guid tripId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdTrip == tripId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all pending participation requests for a driver
        /// </summary>
        /// <param name="driverId">The ID of the driver</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetPendingRequestsForDriverAsync(Guid driverId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Include(pt => pt.Trip)
                .Where(pt => pt.Trip.IdDriver == driverId && pt.IdRequestStatus == 1) // Pending status
                .ToListAsync();
        }

        /// <summary>
        /// Accepts a participation request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <returns>The updated PassengerTrip entity</returns>
        public async Task<PassengerTrip> AcceptRequestAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            // Check if the trip still has available seats
            var hasAvailableSeats = await HasAvailableSeatsAsync(request.IdTrip);
            if (!hasAvailableSeats)
                throw new InvalidOperationException("The trip has no available seats");

            request.IdRequestStatus = 2; // Accepted status

            await _context.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// Rejects a participation request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <param name="reason">The reason for rejection</param>
        /// <returns>The updated PassengerTrip entity</returns>
        public async Task<PassengerTrip> RejectRequestAsync(Guid requestId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required", nameof(reason));

            var request = await _context.PassengerTrips
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            request.IdRequestStatus = 3; // Rejected status
            request.Reason = reason;

            await _context.SaveChangesAsync();

            return request;
        }

        /// <summary>
        /// Checks the waiting list for a trip and moves passengers if seats become available
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <returns>List of PassengerTrip entities that were moved from waiting list to pending</returns>
        public async Task<List<PassengerTrip>> CheckWaitingListAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.PassengerTrips.Where(pt => pt.IdRequestStatus == 2)) // Only count accepted requests
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null)
                throw new ArgumentException("Trip not found", nameof(tripId));

            var availableSeats = trip.AvailableSeats - trip.PassengerTrips.Count;
            if (availableSeats <= 0)
                return new List<PassengerTrip>(); // No available seats

            // Get passengers in waiting list ordered by request date
            var waitingList = await _context.PassengerTrips
                .Where(pt => pt.IdTrip == tripId && pt.IdRequestStatus == 4) // WaitingList status
                .OrderBy(pt => pt.DateRequest)
                .Take(availableSeats)
                .ToListAsync();

            foreach (var request in waitingList)
            {
                request.IdRequestStatus = 1; // Move to Pending status
            }

            await _context.SaveChangesAsync();
            return waitingList;
        }

        /// <summary>
        /// Gets all requests for a passenger
        /// </summary>
        /// <param name="passengerId">The ID of the passenger</param>
        /// <returns>List of PassengerTrip entities</returns>
        public async Task<List<PassengerTrip>> GetRequestsByPassengerAsync(Guid passengerId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdPassenger == passengerId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a specific passenger trip request
        /// </summary>
        /// <param name="requestId">The ID of the request</param>
        /// <returns>The PassengerTrip entity</returns>
        public async Task<PassengerTrip> GetRequestByIdAsync(Guid requestId)
        {
            var request = await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .Include(pt => pt.Passenger)
                    .ThenInclude(p => p.User)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .FirstOrDefaultAsync(pt => pt.Id == requestId);

            if (request == null)
                throw new ArgumentException("Request not found", nameof(requestId));

            return request;
        }
    }
}