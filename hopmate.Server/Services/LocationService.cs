using hopmate.Server.Data;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace hopmate.Server.Services
{
    public class LocationService
    {
        private readonly ApplicationDbContext _context;

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Location> GetOrCreateAsync(LocationDto dto)
        {
            var existing = await _context.Locations
                .FirstOrDefaultAsync(l => l.Address == dto.Address && l.PostalCode == dto.PostalCode);

            if (existing != null) return existing;

            var location = new Location
            {
                Id = Guid.NewGuid(),
                Address = dto.Address,
                PostalCode = dto.PostalCode
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return location;
        }
    }
}
