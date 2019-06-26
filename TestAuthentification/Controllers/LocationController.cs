using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestAuthentification.Models;
using TestAuthentification.Resources;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;
using TestAuthentification.ViewModels.Comments;
using TestAuthentification.ViewModels.Location;
using TestAuthentification.ViewModels.Vehicle;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly BookYourCarContext _context;
        private readonly AuthService _authService;

        private ILogger _logger;

        public LocationController(BookYourCarContext context)
        {
            _context = context;
            _authService = new AuthService(context);
        }

        // GET: api/Locations
        [HttpGet, Route("ManageLocations")]
        public async Task<IActionResult> GetAllLocation()
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
                    return Ok(locations.ToList());
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        // GET: api/Locations
        [HttpGet]
        public async Task<IActionResult> GetLocation()
        {
            string token = GetToken();

            if (TokenService.ValidateToken(token))
            {
                User connectedUser = _authService.GetUserConnected(token);

                var listLocation = await _context.Location.Where(l => l.LocUserId == connectedUser.UserId).ToListAsync();

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
                }
                return Ok(locations.ToList());
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

            try
            {
                List<Pole> poles = _context.Pole.ToList();
                User user = _context.User.FirstOrDefault(x => x.UserId == location.LocUserId);
                Comments comment = _context.Comments.FirstOrDefault(c => c.CommentLocId == location.LocId);

                //Initialize ViewModel with params present everytime
                ManageLocationViewModel locVM = new ManageLocationViewModel()
                {
                    LocId = location.LocId,
                    LocState = GetLocationStateTrad(location.LocState),
                    LocStateId = location.LocState,
                    User = user.UserFirstname + " " + user.UserName,
                    PoleStart = poles.Where(p => p.PoleId == location.LocPoleIdstart).First().PoleName,
                    PoleEnd = poles.Where(p => p.PoleId == location.LocPoleIdend).First().PoleName,
                    DateStart = location.LocDatestartlocation,
                    DateEnd = location.LocDateendlocation,
                    Comment = comment.CommentText
                };

                switch (location.LocState)
                {
                    case (sbyte)Enums.LocationState.Asked:
                        locVM.AvailableVehicles = GetAvailableVehiculeForLocation(location);
                        break;
                    case (sbyte)Enums.LocationState.InProgress:
                        locVM.SelectedVehicle = GetSelectedVehicle(location);
                        break;
                    case (sbyte)Enums.LocationState.Validated:
                        locVM.SelectedVehicle = GetSelectedVehicle(location);
                        locVM.AvailableVehicles = GetAvailableVehiculeForLocation(location);

                        break;
                    case (sbyte)Enums.LocationState.Rejected:
                        break;
                    case (sbyte)Enums.LocationState.Finished:
                        locVM.SelectedVehicle = GetSelectedVehicle(location);
                        break;
                    case (sbyte)Enums.LocationState.Canceled:
                        break;
                }
                return Ok(locVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", "Erreur lors de la récupération de la réservation.");
                return BadRequest(ModelState);
                throw;
            }
        }


        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation([FromRoute] int id, [FromBody] LocationUpdateVM location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            Location locInProgress = _context.Location.SingleOrDefault(l => l.LocId == id);

            User user = _context.User.SingleOrDefault(u => u.UserId == locInProgress.LocUserId);

            Vehicle vehicle = _context.Vehicle.SingleOrDefault(v => v.VehId == location.VehicleId);

            Pole poleS = _context.Pole.SingleOrDefault(p => p.PoleId == locInProgress.LocPoleIdstart);

            Pole poleE = _context.Pole.SingleOrDefault(p => p.PoleId == locInProgress.LocPoleIdend);

            locInProgress.LocVehId = location.VehicleId;

            locInProgress.LocState = (sbyte)Enums.LocationState.Validated;

            _context.Update(locInProgress);

            _context.SaveChanges();

            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationValidation);
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
            myFiles = myFiles.Replace("%%DEBUTLOCATION%%", locInProgress.LocDatestartlocation.ToLongDateString());
            myFiles = myFiles.Replace("%%FINLOCATION%%", locInProgress.LocDateendlocation.ToLongDateString());
            myFiles = myFiles.Replace("%%DEPARTPOLE%%", poleS.PoleName);
            myFiles = myFiles.Replace("%%FINPOLE%%", poleE.PoleName);

            myFiles = myFiles.Replace("%%MODELE%%", vehicle.VehModel);
            myFiles = myFiles.Replace("%%MARQUE%%", vehicle.VehBrand);
            myFiles = myFiles.Replace("%%IMMATRICULATION%%", vehicle.VehRegistration);
            myFiles = myFiles.Replace("%%KM%%", vehicle.VehKm.ToString());
            
            var response = await EmailService.SendEmailAsync("Validation de votre réservation - BookYourCar", myFiles, user.UserEmail);
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite sur l'envoi de mail de confirmation mais la validation de la réservation a bien été prise en compte.");
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token = GetToken();

            // cas ou l'administrateur rejette la demande
            if (TokenService.ValidateTokenWhereIsAdmin(token))
            {
                var location = await _context.Location.FindAsync(id);
                if (location == null)
                {
                    return NotFound();
                }

                // si la réservation à un véhicule affecté il faut l'enlever 
                if (location.LocVehId.HasValue)
                {
                    //TODO CORRECT
                    // on change l'état de disponiblite du vehicule à Available
                    var locationVehicule = _context.Vehicle.SingleOrDefault(x => x.VehId == location.LocVehId);
                    locationVehicule.VehState = (sbyte)Enums.VehiculeState.Available;
                    // on desafecte le vehicule
                    location.LocVehId = null;
                    _context.Update(locationVehicule);
                    _context.SaveChanges();
                }
                _context.Location.Update(location);
                _context.SaveChanges();

                return Ok();
            }

            // cas ou l'utilisateur annule sa demande
            else if (TokenService.ValidateToken(token))
            {
                var location = await _context.Location.FindAsync(id);
                if (location == null)
                {
                    return NotFound();
                }
                // si la réservation à un véhicule affecté il faut l'enlever 
                if (location.LocVehId.HasValue)
                {
                    // on change l'état de disponiblite du vehicule à Available
                    var locationVehicule = _context.Vehicle.SingleOrDefault(x => x.VehId == location.LocVehId);
                    locationVehicule.VehState = (sbyte)Enums.VehiculeState.Available;
                    // on desafecte le vehicule
                    location.LocVehId = null;
                    _context.Update(locationVehicule);
                }

                location.LocState = (sbyte)Enums.LocationState.Canceled;
                _context.Location.Update(location);
                _context.SaveChanges();
                return Ok();
            }

            return Unauthorized();
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

            if (UserAlreadyHaveALocation(model, user))
            {
                ModelState.AddModelError("Error", "Il existe déjà une location enregistrée à votre nom durant cette période.");
                return BadRequest(ModelState);
            }
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

#if !DEBUG
                PoleService servicePole = new PoleService(_context);
                var poleDepart = servicePole.GetPole(location.LocPoleIdstart).PoleName;
                var poleArrive = servicePole.GetPole(location.LocPoleIdend).PoleName;
                string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationAsk);
                
                myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
                myFiles = myFiles.Replace("%%DEBUTLOCATION%%", location.LocDatestartlocation.ToLongDateString());
                myFiles = myFiles.Replace("%%FINLOCATION%%", location.LocDateendlocation.ToLongDateString());
                myFiles = myFiles.Replace("%%DEPARTPOLE%%", poleDepart);
                myFiles = myFiles.Replace("%%FINPOLE%%", poleArrive);
                var response = await EmailService.SendEmailAsync("Vous venez de demander une Location - BookYourCar", myFiles, user.UserEmail);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("Error",
                    "Une erreur s'est produite sur l'envoi de mail de confirmation mais la validation de la réservation a bien été prise en compte.");
                return BadRequest(ModelState);
                }
#endif



                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", "Une erreur est survenue lors de votre demande de location.");
                return BadRequest(ModelState);
            }
        }

        #region Private methods
        private bool UserAlreadyHaveALocation(LocationViewModel model, User user)
        {
            user.Location = _context.Location.Where(l => l.LocUserId == user.UserId).ToList();

            foreach (Location location in user.Location)
            {
                if (location.LocDatestartlocation <= model.DateDebutResa && location.LocDateendlocation >= model.DateFinResa)
                {
                    return true;
                }
                if (location.LocDateendlocation >= model.DateDebutResa && location.LocDateendlocation <= model.DateFinResa)
                {
                    return true;
                }
                if (location.LocDatestartlocation >= model.DateDebutResa && location.LocDatestartlocation <= model.DateFinResa)
                {
                    return true;
                }
            }
            return false;
        }
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
        private List<AvailableVehiculeViewModel> GetAvailableVehiculeForLocation(Location location)
        {
            List<Vehicle> vehicleList = _context.Vehicle.ToList();
            List<Vehicle> selectedVehicles = new List<Vehicle>();

            foreach (Vehicle vehicle in vehicleList)
            {
                vehicle.Location = _context.Location.Where(l => l.LocVehId == vehicle.VehId).ToList();
            }

            foreach (Vehicle vehicle in vehicleList)
            {

                // Si aucune loc ne respecte les 3 conditions suivantes, on ajoute le vehicule à la liste 
                // loc commence avant et fini après la loc demandée
                // loc fini pendant la loc demandée
                // loc commence pendant la loc demandée
                if (vehicle.Location.Where(l => l.LocDatestartlocation < location.LocDatestartlocation && l.LocDateendlocation > location.LocDateendlocation
                                             || l.LocDateendlocation < location.LocDatestartlocation && l.LocDateendlocation > location.LocDatestartlocation
                                             || l.LocDatestartlocation > location.LocDatestartlocation && l.LocDatestartlocation < location.LocDateendlocation) == null)
                {
                    selectedVehicles.Add(vehicle);
                }
            }

            if (selectedVehicles.Count > 0)
            {
                Location lastLoc = new Location();

                foreach (Vehicle vehicle in selectedVehicles)
                {
                    List<Location> locs = vehicle.Location.Where(l => l.LocDateendlocation < location.LocDatestartlocation).ToList();
                    lastLoc = locs.OrderByDescending(l => l.LocDateendlocation).First();
                    if (lastLoc.LocPoleIdend != location.LocPoleIdend)
                    {
                        selectedVehicles.Remove(vehicle);
                    }
                }
            }

            return selectedVehicles;
        }
        private VehicleDetailsViewModel GetSelectedVehicle(Location location)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}