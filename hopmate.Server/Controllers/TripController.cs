using hopmate.Server.DTOs;
using hopmate.Server.Models.Dto;
using hopmate.Server.Models.Entities;
using hopmate.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly TripService _tripService;

        public TripController(TripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost]
        public async Task<ActionResult<Trip>> CreateTrip([FromBody] TripDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest("Trip data is invalid.");
            }

            var createdTrip = await _tripService.CreateTripAsync(tripDto);
            return CreatedAtAction(nameof(GetTrip), new { id = createdTrip.Id }, createdTrip);
        }

        [HttpGet]
        public async Task<ActionResult<List<Trip>>> GetTrips()
        {
            var trips = await _tripService.GetTripsAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trip>> GetTrip(Guid id)
        {
            var trip = await _tripService.GetTripsAsync();
            var existingTrip = trip.Find(t => t.Id == id);

            if (existingTrip == null)
            {
                return NotFound("Trip not found.");
            }

            return Ok(existingTrip);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Trip>> UpdateTrip(Guid id, [FromBody] TripDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest("Trip data is invalid.");
            }

            var updatedTrip = await _tripService.UpdateTripAsync(id, tripDto);

            if (updatedTrip == null)
            {
                return NotFound("Trip not found.");
            }

            return Ok(updatedTrip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var result = await _tripService.DeleteTripAsync(id);

            if (!result)
            {
                return NotFound("Trip not found.");
            }

            return NoContent();
        }

        [HttpGet("driver/{driverId}")]
        public async Task<ActionResult<Driver>> GetDriver(Guid driverId)
        {
            var driver = await _tripService.GetDriverAsync(driverId);
            if (driver == null)
            {
                return NotFound("Driver not found.");
            }

            return Ok(driver);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(Guid vehicleId)
        {
            var vehicle = await _tripService.GetVehicleAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(vehicle);
        }

        [HttpGet("status/{statusId}")]
        public async Task<ActionResult<TripStatus>> GetTripStatus(Guid statusId)
        {
            var tripStatus = await _tripService.GetTripStatusAsync(statusId);
            if (tripStatus == null)
            {
                return NotFound("Trip status not found.");
            }

            return Ok(tripStatus);
        }
    }
}
