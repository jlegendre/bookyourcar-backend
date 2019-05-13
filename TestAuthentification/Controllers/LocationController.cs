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
using TestAuthentification.ViewModels;
using TestAuthentification.ViewModels.Comments;
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
                            locVM.LocationId = loc.LocId;
                            locVM.DateDebutResa = loc.LocDatestartlocation;
                            locVM.DateFinResa = loc.LocDateendlocation;

                            User user = _context.User.Where(u => u.UserId == loc.LocUserId).First();
                            locVM.UserFriendlyName = String.Format("{0} {1}", user.UserFirstname, user.UserName);

                            Pole poleStart = _context.Pole.Where(p => p.PoleId == loc.LocPoleIdstart).First();
                            locVM.PoleDepart = poleStart.PoleName;
                            Pole poleEnd = _context.Pole.Where(p => p.PoleId == loc.LocPoleIdend).First();
                            locVM.PoleDestination = poleEnd.PoleName;

                            if (loc.LocVehId != null)
                            {
                                Vehicle vehicle = _context.Vehicle.Where(v => v.VehId == loc.LocVehId).First();
                                locVM.VehicleFriendlyName = String.Format("{0} {1}", vehicle.VehBrand, vehicle.VehModel);
                            }
                            else
                            {
                                locVM.VehicleFriendlyName = "Pas de vehicule associé";
                            }


                            locVM.LocationState = GetLocationStateTrad(loc.LocState);
                            locVM.LocationStateId = loc.LocState;

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
                        User user = _context.User.Where(u => u.UserId == connectedUser.UserId).First();

                        foreach (Location loc in listLocation)
                        {
                            LocationListViewModel locVM = new LocationListViewModel();
                            locVM.LocationId = loc.LocId;
                            locVM.DateDebutResa = loc.LocDatestartlocation;
                            locVM.DateFinResa = loc.LocDateendlocation;

                            locVM.UserFriendlyName = String.Format("{0} {1}", user.UserFirstname, user.UserName);

                            Pole poleStart = _context.Pole.Where(p => p.PoleId == loc.LocPoleIdstart).First();
                            locVM.PoleDepart = poleStart.PoleName;
                            Pole poleEnd = _context.Pole.Where(p => p.PoleId == loc.LocPoleIdend).First();
                            locVM.PoleDestination = poleEnd.PoleName;

                            if (loc.LocVehId != null)
                            {
                                Vehicle vehicle = _context.Vehicle.Where(v => v.VehId == loc.LocVehId).First();
                                locVM.VehicleFriendlyName = String.Format("{0} {1}", vehicle.VehBrand, vehicle.VehModel);
                            }
                            else
                            {
                                locVM.VehicleFriendlyName = "Pas de vehicule associé";
                            }

                            locVM.LocationState = GetLocationStateTrad(loc.LocState);

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
            if (location == null)
            {
                return NotFound();
            }

            var userInfo = _context.User.FirstOrDefault(x => x.UserId == location.LocUserId);
            //lier les commentaires à la location
            List<CommentsViewModel> commentsList = _context.Comments.Where(c=>c.CommentLocId == location.LocId).Select(x => new CommentsViewModel()
            {
                UserId = x.CommentUserId.GetValueOrDefault(),
                DatePublication = x.CommentDate,
                FriendlyName = string.Format("{0} {1}", userInfo.UserFirstname, userInfo.UserName),
                Text = x.CommentText
            }).ToList();


            //_context.Comments.Where(c => c.CommentId == location.LocId);
            LocationDetailsViewModel locDetailVm = new LocationDetailsViewModel()
            {
                UserId = _context.User.SingleOrDefault(u => u.UserId == location.LocUserId).UserId,
                CommentsList = commentsList,
                DateDebutResa = location.LocDatestartlocation,
                DateFinResa = location.LocDateendlocation,
                LocationState = GetLocationStateTrad(location.LocState),
                LocationStateId = location.LocState,
                PoleDestination = _context.Pole.FirstOrDefault(p => p.PoleId == location.LocPoleIdend).PoleName,
                PoleDepart = _context.Pole.FirstOrDefault(p => p.PoleId == location.LocPoleIdend).PoleName

            };

            //afficher la voiture ou liste voitures disponibles
            if (location.LocState == (sbyte)Enums.LocationState.Asked)
            {
                locDetailVm.AvailableVehicle = new List<VehiculeViewModel>()
                {
                    new VehiculeViewModel(){VehBrand = "Test", VehModel = "Bouchon"},
                    new VehiculeViewModel(){VehBrand = "Ferrari", VehModel = "Rouge"},
                    new VehiculeViewModel(){VehBrand = "Twingo", VehModel = "Verte"}
                };
            }
            else
            {
                var vehicule = _context.Vehicle.FirstOrDefault(v => v.VehId == location.LocVehId);
                locDetailVm.SelectedVehicle = new VehiculeViewModel()
                {
                    PoleName = _context.Pole.FirstOrDefault(p => p.PoleId == location.LocPoleIdend).PoleName,
                    VehModel = vehicule.VehModel,
                    VehId = vehicule.VehId,
                    VehBrand = vehicule.VehBrand,
                    VehDatemec = vehicule.VehDatemec,
                    VehKm = vehicule.VehKm,
                    VehNumberplace = vehicule.VehNumberplace,
                    VehRegistration = vehicule.VehRegistration,
                    VehTypeEssence = vehicule.VehTypeEssence,
                    VehColor = vehicule.VehColor,
                    VehIsactive = vehicule.VehIsactive
                };
            }
            //_context.Vehicle.FirstOrDefault(v => v.VehId == location.LocVehId);
            //Commentaires associés à la location

            return Ok(locDetailVm);
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


        private string GetLocationStateTrad(sbyte locState)
        {
            Enums.LocationState locSt = (Enums.LocationState)locState;
            string locationStateTrad = "";
            switch (locSt)
            {
                case Enums.LocationState.Asked:
                    locationStateTrad = "Demandée";
                    break;
                case Enums.LocationState.InProgress:
                    locationStateTrad = "En cours";
                    break;
                case Enums.LocationState.Validated:
                    locationStateTrad = "Validée";
                    break;
                case Enums.LocationState.Rejected:
                    locationStateTrad = "Refusée";
                    break;
                case Enums.LocationState.Finished:
                    locationStateTrad = "Terminée";
                    break;
                case Enums.LocationState.Canceled:
                    locationStateTrad = "Annulée";
                    break;
            }
            return locationStateTrad;
        }
    }
}