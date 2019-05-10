using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.Resources;
using TestAuthentification.Services;
using TestAuthentification.ViewModels.Location;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly A5dContext _context;
        private readonly AuthService _authService;

        public LocationController(A5dContext context)
        {
            _context = context;
            _authService = new AuthService(context);
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<IActionResult> GetLocation()
        {
            string token = GetToken();

            if (TokenService.ValidateToken(token))
            {
                User connectedUser = _authService.GetUserConnected(token);

                if (connectedUser.UserRight.RightLabel == Enums.Roles.Admin.ToString())
                {
                    var listLocation = await _context.Location.ToListAsync();

                    List<LocationListViewModel> locations = new List<LocationListViewModel>();

                    if (listLocation.Count > 0)
                    {
                        foreach (Location loc in listLocation)
                        {
                            LocationListViewModel locVM = new LocationListViewModel();
                            locVM.DateDebutResa = loc.LocDatestartlocation;
                            locVM.DateFinResa = loc.LocDateendlocation;
                            locVM.PoleIdDepart = loc.LocPoleIdstart;
                            locVM.PoleIdDestination = loc.LocPoleIdend;
                            locVM.VehId = loc.LocVehId;

                            locations.Add(locVM);
                        }
                        return Ok(locations.ToList());
                    }
                    var emptyLocation = new Dictionary<string, string>();
                    emptyLocation.Add("message", "Il n'y a pas de locations.");
                    return Ok(emptyLocation);
                }
                else
                {
                    var listLocation = await _context.Location.Where(loc => loc.LocUserId == connectedUser.UserId).ToListAsync();

                    List<LocationListViewModel> locations = new List<LocationListViewModel>();

                    if (listLocation.Count > 0)
                    {
                        foreach (Location loc in listLocation)
                        {
                            LocationListViewModel locVM = new LocationListViewModel();
                            locVM.DateDebutResa = loc.LocDatestartlocation;
                            locVM.DateFinResa = loc.LocDateendlocation;
                            locVM.PoleIdDepart = loc.LocPoleIdstart;
                            locVM.PoleIdDestination = loc.LocPoleIdend;
                            locVM.VehId = loc.LocVehId;

                            locations.Add(locVM);
                        }
                        return Ok(locations.ToList());
                    }
                    var emptyLocation = new Dictionary<string, string>();
                    emptyLocation.Add("message", "Il n'y a pas de locations.");
                    return Ok(emptyLocation);
                }
            }
            else
            {
                return Unauthorized();
            }

        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Location location = await _context.Location.FindAsync(id);
            User user = await _context.User.FindAsync(location.LocUserId);
            Pole startPole = await _context.Pole.FindAsync(location.LocPoleIdstart);
            Pole endPole = await _context.Pole.FindAsync(location.LocPoleIdstart);

            if(location.LocState == (sbyte)Enums.LocationState.Asked)
            {
                List<Vehicle> vehicles = (List<Vehicle>)_context.Vehicle.FindAsync().ToAsyncEnumerable();
            }

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation([FromRoute] int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.LocId)
            {
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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

        // POST: api/Locations
        [HttpPost]
        public async Task<IActionResult> PostLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Location.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.LocId }, location);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var location = await _context.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Location.Remove(location);
            await _context.SaveChangesAsync();

            return Ok(location);
        }

        /// <summary>
        /// Demande de nouvelle location pour un utilisateur
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: api/Location/AskLocation
        [HttpPost, Route("AskLocation")]
        public async Task<IActionResult> AskLocation([FromBody] LocationViewModel model)
        {
            var token = GetToken();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TokenService.ValidateToken(token)) return Unauthorized();


            AuthService service = new AuthService(_context);
            User user = service.GetUserConnected(token);


            Location location = new Location();

            // information commentaire
            Comments comment = new Comments();
            comment.CommentDate = DateTime.Now;
            comment.CommentText = model.Comments;
            comment.CommentUserId = user.UserId;


            // information location
            location.LocDatestartlocation = model.DateDebutResa;
            location.LocDateendlocation = model.DateFinResa;
            location.LocPoleIdstart = model.PoleIdDepart;
            location.LocPoleIdend = model.PoleIdDestination;
            location.LocUserId = user.UserId;
            location.LocState = Convert.ToSByte(Enums.LocationState.Asked);


            try
            {
                // un commentaire a besoin d'être d'abord rattacher a une location
                _context.Location.Add(location);
                await _context.SaveChangesAsync();
                comment.CommentLocId = location.LocId;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                PoleService servicePole = new PoleService(_context);
                var poleDepart = servicePole.GetPole(location.LocPoleIdstart).PoleName;
                var poleArrive = servicePole.GetPole(location.LocPoleIdend).PoleName;

#if !DEBUG
                await EmailService.SendEmailAsync("Vous venez de demander une Location", String.Format(ConstantsEmail.LocationAsk, user.UserFirstname, location.LocDatestartlocation, location.LocDateendlocation, poleDepart, poleArrive), user.UserEmail);
#endif

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
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


        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.LocId == id);
        }
    }
}