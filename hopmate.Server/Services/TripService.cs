using hopmate.Server.Data;
using Microsoft.EntityFrameworkCore;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Services
{
    public class TripService
    {
        private readonly ApplicationDbContext _context;

        public TripService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Trip> CreateTripAsync(TripDto tripDto)
        {
            var trip = new Trip
            {
                DtDeparture = tripDto.DtDeparture,
                DtArrival = tripDto.DtArrival,
                AvailableSeats = tripDto.AvailableSeats,
                IdDriver = tripDto.IdDriver,
                IdVehicle = tripDto.IdVehicle,
                IdStatusTrip = tripDto.IdStatusTrip
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return trip;
        }

        public async Task<List<Trip>> GetTripsAsync()
        {
            return await _context.Trips
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .Include(t => t.TripStatus)
                .ToListAsync();
        }

        public async Task<Trip?> UpdateTripAsync(Guid id, TripDto tripDto)
        {
            var existingTrip = await _context.Trips.FindAsync(id);
            if (existingTrip == null)
            {
                return null;
            }

            existingTrip.DtDeparture = tripDto.DtDeparture;
            existingTrip.DtArrival = tripDto.DtArrival;
            existingTrip.AvailableSeats = tripDto.AvailableSeats;
            existingTrip.IdDriver = tripDto.IdDriver;
            existingTrip.IdVehicle = tripDto.IdVehicle;
            existingTrip.IdStatusTrip = tripDto.IdStatusTrip;

            await _context.SaveChangesAsync();
            return existingTrip;
        }

        public async Task<bool> DeleteTripAsync(Guid id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return false;
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Driver?> GetDriverAsync(Guid driverId)
        {
            return await _context.Drivers.FindAsync(driverId);
        }

        public async Task<Vehicle?> GetVehicleAsync(Guid vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }

        public async Task<TripStatus?> GetTripStatusAsync(Guid statusId)
        {
            return await _context.TripStatuses.FindAsync(statusId);
        }
    }
}
