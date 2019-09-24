using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;
using TestAuthentification.Resources;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly BookYourCarContext _context;

        private ILogger _logger;

        public VehicleController(BookYourCarContext context)
        {
            _context = context;
            _context.Pole.ToList();

        }

        // GET: api/Vehicles
        [HttpGet]
        public async Task<IActionResult> GetVehicle()
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            List<Vehicle> listVehicle = _context.Vehicle.ToList();

            if (listVehicle.Count > 0)
            {
                List<VehiculeViewModel> model = listVehicle.Select(x => new VehiculeViewModel()
                {
                    PoleId = x.VehPoleId,
                    VehModel = x.VehModel,
                    PoleName = _context.Pole.Any(p => p.PoleId == x.VehPoleId) ? _context.Pole.SingleOrDefault(p => p.PoleId == x.VehPoleId)?.PoleName : "",
                    VehId = x.VehId,
                    VehBrand = x.VehBrand,
                    VehColor = x.VehColor,
                    VehKm = x.VehKm,
                    VehNumberplace = x.VehNumberplace,
                    VehRegistration = x.VehRegistration,
                    VehTypeEssence = x.VehTypeEssence,
                    VehDatemec = x.VehDatemec,
                    VehIsactive = x.VehIsactive,
                    VehState = x.VehState

                }).Where(x => x.VehState != (int)Enums.VehiculeState.Deleted).ToList();
                return Ok(model.OrderBy(x=>x.VehRegistration));
            }
            return Ok(new List<Vehicle>());

        }

        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if(!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            try
            {
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
                        PoleId = vehicle.VehPoleId,
                        PoleName = _context.Pole.Any(p => p.PoleId == vehicle.VehPoleId) ? _context.Pole.SingleOrDefault(p => p.PoleId == vehicle.VehPoleId)?.PoleName : "",
                        VehState = vehicle.VehState
                    };

                    return Ok(model);
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite." + e.Message);
                return BadRequest(ModelState);
            }

            return NotFound();

        }

        // PUT: api/Vehicles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicle([FromRoute] int id, [FromBody] VehiculeViewModel vehicle)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


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
            vehiculeToModifie.VehState = (sbyte)vehicle.VehState;

            _context.Entry(vehiculeToModifie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!VehicleExists(id))
                {
                    return NotFound();
                }
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite lors de la modification du vehicule : ." + vehicle.VehModel);
                return BadRequest(ModelState);

            }

            return NoContent();
        }

        // POST: api/Vehicles
        [HttpPost]
        public async Task<IActionResult> PostVehicle([FromBody] VehiculeViewModel vehicle)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

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
                VehPoleId = vehicle.PoleId,
                VehState = (sbyte)Enums.VehiculeState.Available,
            };

            try
            {
                _context.Vehicle.Add(vehiculeToAdd);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", "Une erreur est survenue lors de la modification du véhicule. " + ex.Message);
                return BadRequest(ModelState);
            }

        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {
                vehicle.VehState = (sbyte)Enums.VehiculeState.Deleted;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite lors de la suppression du vehicule : . " + vehicle.VehModel + "Erreur :  " + e.Message);
                return BadRequest(ModelState);
            }


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
            try
            {
                var token = Request.Headers["Authorization"].ToString();
                if (token.StartsWith("Bearer"))
                {
                    var tab = token.Split(" ");
                    token = tab[1];
                }

                return token;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                return "";
            }

        }

    }
}