using hopmate.Server.Models.Entities;
using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace hopmate.Server.Services
{
    public class PassengerTripService
    {
        private readonly ApplicationDbContext _context;

        public PassengerTripService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PassengerTrip?> GetRequestByIdAsync(Guid id)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                .Include(pt => pt.Trip)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<List<PassengerTrip>> GetRequestsByTripIdAsync(Guid tripId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Passenger)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdTrip == tripId)
                .ToListAsync();
        }

        public async Task<List<PassengerTrip>> GetRequestsByPassengerIdAsync(Guid passengerId)
        {
            return await _context.PassengerTrips
                .Include(pt => pt.Trip)
                .Include(pt => pt.Location)
                .Include(pt => pt.RequestStatus)
                .Where(pt => pt.IdPassenger == passengerId)
                .ToListAsync();
        }

        public async Task<PassengerTrip> CreateRequestAsync(PassengerTrip request)
        {
            // Verificar se há lugares disponíveis
            var trip = await _context.Trips.FindAsync(request.IdTrip);
            if (trip == null)
                throw new Exception("Trip not found");

            if (trip.AvailableSeats <= 0)
                throw new Exception("No available seats");

            // Verificar se o passageiro já tem um pedido pendente para esta viagem
            var existingRequest = await _context.PassengerTrips
                .FirstOrDefaultAsync(pt =>
                    pt.IdPassenger == request.IdPassenger &&
                    pt.IdTrip == request.IdTrip &&
                    (pt.IdRequestStatus == 1 || pt.IdRequestStatus == 4)); // Pending or WaitingList

            if (existingRequest != null)
                throw new Exception("You already have a pending request for this trip");

            request.IdRequestStatus = 1; // Pending
            request.DateRequest = DateTime.UtcNow;

            _context.PassengerTrips.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<PassengerTrip> UpdateRequestStatusAsync(Guid requestId, int newStatusId, string? reason = null)
        {
            var request = await _context.PassengerTrips.FindAsync(requestId);
            if (request == null)
                throw new Exception("Request not found");

            var trip = await _context.Trips.FindAsync(request.IdTrip);
            if (trip == null)
                throw new Exception("Trip not found");

            // Se está sendo aceito, verificar se ainda há lugares
            if (newStatusId == 2) // Accepted
            {
                if (trip.AvailableSeats <= 0)
                    throw new Exception("No available seats");

                trip.AvailableSeats -= 1;
            }
            // Se está sendo rejeitado e estava aceito antes, liberar o lugar
            else if (newStatusId == 3 && request.IdRequestStatus == 2) // Rejected and was Accepted
            {
                trip.AvailableSeats += 1;
            }

            request.IdRequestStatus = newStatusId;
            request.Reason = reason;

            await _context.SaveChangesAsync();

            return request;
        }
    }
}