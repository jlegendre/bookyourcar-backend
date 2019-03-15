using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly A5dContext _context;

        public RideController(A5dContext context)
        {
            _context = context;
        }

        // GET: api/Rides
        [HttpGet]
        public IEnumerable<Ride> GetRide()
        {
            return _context.Ride;
        }

        // GET: api/Rides/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRide([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ride = await _context.Ride.FindAsync(id);

            if (ride == null)
            {
                return NotFound();
            }

            return Ok(ride);
        }

        // PUT: api/Rides/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRide([FromRoute] int id, [FromBody] Ride ride)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ride.RideId)
            {
                return BadRequest();
            }

            _context.Entry(ride).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rides
        [HttpPost]
        public async Task<IActionResult> PostRide([FromBody] Ride ride)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Ride.Add(ride);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRide", new { id = ride.RideId }, ride);
        }

        // DELETE: api/Rides/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRide([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ride = await _context.Ride.FindAsync(id);
            if (ride == null)
            {
                return NotFound();
            }

            _context.Ride.Remove(ride);
            await _context.SaveChangesAsync();

            return Ok(ride);
        }

        private bool RideExists(int id)
        {
            return _context.Ride.Any(e => e.RideId == id);
        }
    }
}