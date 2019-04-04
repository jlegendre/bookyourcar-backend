using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.ViewModels;
using TestAuthentification.ViewModels.Vehicle;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly A5dContext _context;

        public VehicleController(A5dContext context)
        {
            _context = context;
            _context.Pole.ToList();
            
        }

        // GET: api/Vehicles
        [HttpGet]
        public async Task<IActionResult> GetVehicle()
        {

            List<Vehicle> listVehicle = _context.Vehicle.ToList();

            if (listVehicle.Count < 1)
            {
                var roles = new Dictionary<string, string>();
                roles.Add("message", "Il n'y a pas de véhicules.");
                return Ok(roles);
            }

            

            var model = listVehicle.Select(x => new ListVehiculeViewModel()
            {
                VehModel = x.VehModel,
                Pole = x.VehPole.PoleName,
                VehId = x.VehId,
                VehBrand = x.VehBrand,
                VehColor = x.VehColor,
                VehKm =  x.VehKm,
                VehNumberplace = x.VehNumberplace,
                VehRegistration = x.VehRegistration,
                VehTypeEssence = x.VehTypeEssence
            });

            return Ok(model.ToList());
        }

        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicle.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        // PUT: api/Vehicles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle([FromRoute] int id, [FromBody] VehiculeViewModel vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculeToModifie = await _context.Vehicle.FindAsync(id);
            vehiculeToModifie.VehDatemec = vehicle.VehDatemec;
            vehiculeToModifie.VehKm = vehicle.VehKm;
            vehiculeToModifie.VehNumberplace = vehicle.VehNumberplace;
            vehiculeToModifie.VehBrand = vehicle.VehBrand;
            vehiculeToModifie.VehColor = vehicle.VehColor;
            vehiculeToModifie.VehModel = vehicle.VehModel;
            vehiculeToModifie.VehRegistration = vehicle.VehRegistration;
            vehiculeToModifie.VehTypeEssence = vehicle.VehTypeEssence;
            vehiculeToModifie.VehIsactive = vehicle.VehIsactive;

            _context.Entry(vehiculeToModifie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
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

        // POST: api/Vehicles
        [HttpPost]
        public async Task<IActionResult> PostVehicle([FromBody] VehiculeViewModel vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var vehiculeToAdd = new Vehicle()
            {
                VehBrand = vehicle.VehBrand,
                VehColor = vehicle.VehColor,
                VehModel = vehicle.VehModel,
                VehRegistration = vehicle.VehRegistration,
                VehTypeEssence = vehicle.VehTypeEssence,
                VehDatemec = vehicle.VehDatemec,
                VehKm = vehicle.VehKm,
                VehNumberplace = vehicle.VehNumberplace,
            };

            _context.Vehicle.Add(vehiculeToAdd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehicle", new { id = vehiculeToAdd.VehId }, vehiculeToAdd);
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicle.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.VehId == id);
        }
    }
}