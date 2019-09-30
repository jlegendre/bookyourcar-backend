using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TestAuthentification.Models;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoleController : ControllerBase
    {
        private readonly BookYourCarContext _context;

        public PoleController(BookYourCarContext context)
        {
            _context = context;
        }

        // GET: api/Poles
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPole()
        {

            var listPole = await _context.Pole.ToListAsync();
            if (listPole.Count > 0)
            {
                var model = listPole.Select(x => new PoleViewModel()
                {
                    PoleName = x.PoleName,
                    PoleAddress = x.PoleAddress,
                    PoleCity = x.PoleCity,
                    PoleCp = x.PoleCp,
                    PoleId = x.PoleId
                });

                return Ok(model.OrderBy(x=>x.PoleCity).ToList());
            }
            return Ok(new List<Pole>());

        }

        // GET: api/Poles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPole([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();
            try
            {
                var pole = await _context.Pole.FindAsync(id);
                if (pole != null)
                {
                    var model = new PoleViewModel()
                    {
                        PoleName = pole.PoleName,
                        PoleId = pole.PoleId,
                        PoleAddress = pole.PoleAddress,
                        PoleCp = pole.PoleCp,
                        PoleCity = pole.PoleCity
                    };
                    return Ok(model);
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                throw;
            }

            return NotFound();

        }

        // PUT: api/Poles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPole([FromRoute] int id, [FromBody] PoleViewModel poleModel)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


            var poleToModifie = await _context.Pole.FindAsync(id);

            poleToModifie.PoleAddress = poleModel.PoleAddress;
            poleToModifie.PoleCity = poleModel.PoleCity;
            poleToModifie.PoleCp = poleModel.PoleCp;
            poleToModifie.PoleName = poleModel.PoleName;

            _context.Entry(poleToModifie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!PoleExists(id))
                {
                    return NotFound();
                }
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite lors de la modification du pole : ." + poleModel.PoleName);
                return BadRequest(ModelState);

            }

            return NoContent();
        }

        // POST: api/Poles
        [HttpPost]
        public async Task<IActionResult> PostPole([FromBody] PoleViewModel poleModel)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


            var pole = new Pole()
            {
                PoleAddress = poleModel.PoleAddress,
                PoleCity = poleModel.PoleCity,
                PoleCp = poleModel.PoleCp,
                PoleName = poleModel.PoleName
            };
            try
            {
                _context.Pole.Add(pole);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite lors de l'ajout du pole : ." + poleModel.PoleName);
                return BadRequest(ModelState);
            }

            return CreatedAtAction("GetPole", new { id = poleModel.PoleId }, poleModel);
        }

        // DELETE: api/Poles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePole([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            var pole = await _context.Pole.FindAsync(id);
            if (pole == null)
            {
                return NotFound();
            }

            try
            {
                if(_context.Vehicle.Count(v=> v.VehPoleId == id) != 0)
                {
                    throw new Exception("Un ou plusieurs véhicules sont affectés à ce pôle, la suppression est donc impossible");
                }
                if (_context.User.Count(u => u.UserPoleId == id) != 0)
                {
                    throw new Exception("Un ou plusieurs utilisateurs sont affectés à ce pôle, la suppression est donc impossible");
                }
                if (_context.Location.Count(l => l.LocPoleIdend == id || l.LocPoleIdstart == id) != 0)
                {
                    throw new Exception("Une ou plusieurs locations sont liées à ce pôle, la suppression est donc impossible");
                }
                _context.Pole.Remove(pole);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite lors de la suppression du pole : " + pole.PoleName);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        private bool PoleExists(int id)
        {
            return _context.Pole.Any(e => e.PoleId == id);
        }


        #region utilitaire Token
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


        #endregion
    }
}