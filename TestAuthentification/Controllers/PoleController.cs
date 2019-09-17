using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                return Ok(model.ToList());
            }
            var poles = new Dictionary<string, string>();
            poles.Add("message", "Il n'y a pas de Poles.");
            return Ok(poles);

        }

        // GET: api/Poles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPole([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token)) return Unauthorized();


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
            return NotFound();

        }

        // PUT: api/Poles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPole([FromRoute] int id, [FromBody] PoleViewModel poleModel)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token)) return Unauthorized();


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
            catch (DbUpdateConcurrencyException)
            {
                if (!PoleExists(id))
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

        // POST: api/Poles
        [HttpPost]
        public async Task<IActionResult> PostPole([FromBody] PoleViewModel poleModel)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token)) return Unauthorized();


            var pole = new Pole()
            {
                PoleAddress = poleModel.PoleAddress,
                PoleCity = poleModel.PoleCity,
                PoleCp = poleModel.PoleCp,
                PoleName = poleModel.PoleName
            };

            _context.Pole.Add(pole);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPole", new { id = pole.PoleId }, pole);
        }

        // DELETE: api/Poles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePole([FromRoute] int id)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token)) return Unauthorized();
            
            var pole = await _context.Pole.FindAsync(id);
            if (pole == null)
            {
                return NotFound();
            }

            _context.Pole.Remove(pole);
            await _context.SaveChangesAsync();

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