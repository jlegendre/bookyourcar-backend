using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

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

            List<Vehicle> listVehicle = await _context.Vehicle.ToListAsync();

            if (listVehicle.Count > 0)
            {
                var model = listVehicle.Select(x => new VehiculeViewModel()
                {
                    VehModel = x.VehModel,
                    PoleName = x.VehPole.PoleName,
                    VehId = x.VehId,
                    VehBrand = x.VehBrand,
                    VehColor = x.VehColor,
                    VehKm = x.VehKm,
                    VehNumberplace = x.VehNumberplace,
                    VehRegistration = x.VehRegistration,
                    VehTypeEssence = x.VehTypeEssence,
                    VehDatemec = x.VehDatemec,
                    VehIsactive = x.VehIsactive
                });
                return Ok(model.ToList());
            }
            var roles = new Dictionary<string, string>();
            roles.Add("message", "Il n'y a pas de véhicules.");
            return Ok(roles);

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
            if (vehicle != null)
            {
                VehiculeViewModel model = new VehiculeViewModel
                {
                    VehId = vehicle.VehId,
                    VehBrand = vehicle.VehBrand,
                    VehColor = vehicle.VehColor,
                    VehDatemec = vehicle.VehDatemec,
                    VehIsactive = vehicle.VehIsactive,
                    VehKm = vehicle.VehKm,
                    VehModel = vehicle.VehModel,
                    VehNumberplace = vehicle.VehNumberplace,
                    VehRegistration = vehicle.VehRegistration,
                    VehTypeEssence = vehicle.VehTypeEssence,
                    PoleName = vehicle.VehPole.PoleName,
                };

                return Ok(model);
            }
            return NotFound();

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
            vehiculeToModifie.VehPole.PoleName = vehicle.PoleName;

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
                VehIsactive = vehicle.VehIsactive,
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

            return Ok();
        }

        

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.VehId == id);
        }

        /// <summary>
        /// permet de récuperer le token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            var token = Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer"))
            {
                var tab = token.Split(" ");
                token = tab[1];
            }

            return token;
        }


    }
}