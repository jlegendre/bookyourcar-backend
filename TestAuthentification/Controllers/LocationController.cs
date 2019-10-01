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
using TestAuthentification.ViewModels.Location;
using TestAuthentification.ViewModels.Planning;
using TestAuthentification.ViewModels.Vehicle;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly BookYourCarContext _context;
        private readonly AuthService _authService;
        private LocationService _locServ;

        private ILogger _logger;

        public LocationController(BookYourCarContext context)
        {
            _context = context;
            _authService = new AuthService(context);
            _locServ = new LocationService(context);
        }

        // GET: api/Locations
        [HttpGet, Route("ManageLocations")]
        public async Task<IActionResult> GetAllLocation()
        {
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


            User connectedUser = _authService.GetUserConnected(token);

            if (connectedUser.UserRight.RightLabel == Enums.Roles.Admin.ToString())
            {
                List<LocationListViewModel> locations = await _locServ.GetAllLocationAsync();
                List<LocationListViewModel> locationsAsked = await _locServ.GetAllLocationInWaitingAsync();

                AllLocationAndLocationAsked modelToReturn = new AllLocationAndLocationAsked();
                modelToReturn.AllLocations = locations;
                modelToReturn.LocationsAsked = locationsAsked;

                return Ok(modelToReturn);
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
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            User connectedUser = _authService.GetUserConnected(token);

            var listLocation = await _context.Location.Where(l => l.LocUserId == connectedUser.UserId).ToListAsync();

            List<LocationListViewModel> locations = new List<LocationListViewModel>();

            if (listLocation.Count > 0)
            {
                foreach (Location loc in listLocation)
                {
                    LocationListViewModel locVM = new LocationListViewModel();
                    locVM.LocationId = loc.LocId;
                    locVM.DateDebutResa = loc.LocDatestartlocation.ToString("dd/MM/yyyy");
                    locVM.DateFinResa = loc.LocDateendlocation.ToLocalTime().ToString("dd/MM/yyyy");

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


                    locVM.LocationState = Enums.GetLocationStateTrad(loc.LocState);
                    locVM.LocationStateId = loc.LocState;

                    locations.Add(locVM);
                }
            }
            return Ok(locations.OrderByDescending(x => x.DateDebutResa).ToList());

        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation([FromRoute] int id)
        {
            #region Prerequisites
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Location location = await _context.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            #endregion

            try
            {
                List<Pole> poles = _context.Pole.ToList();
                User user = _context.User.FirstOrDefault(x => x.UserId == location.LocUserId);
                Comments comment = _context.Comments.FirstOrDefault(c => c.CommentLocId == location.LocId);

                //Initialize ViewModel with params present everytime
                ManageLocationViewModel locVM = new ManageLocationViewModel()
                {
                    LocId = location.LocId,
                    LocState = Enums.GetLocationStateTrad(location.LocState),
                    LocStateId = location.LocState,
                    User = user.UserFirstname + " " + user.UserName,
                    PoleStart = poles.Where(p => p.PoleId == location.LocPoleIdstart).First().PoleName,
                    PoleEnd = poles.Where(p => p.PoleId == location.LocPoleIdend).First().PoleName,
                    DateStart = location.LocDatestartlocation,
                    DateEnd = location.LocDateendlocation,
                    Comment = comment == null ? "" : comment.CommentText
                };

                LocationService locServ = new LocationService(_context);

                switch (location.LocState)
                {
                    case (sbyte)Enums.LocationState.Asked:
                        locVM.AvailableVehicles = locServ.GetAvailableVehiculeForLocation(location);
                        break;
                    case (sbyte)Enums.LocationState.InProgress:
                        locVM.SelectedVehicle = GetSelectedVehicle(location);
                        break;
                    case (sbyte)Enums.LocationState.Validated:
                        locVM.SelectedVehicle = GetSelectedVehicle(location);
                        locVM.AvailableVehicles = locServ.GetAvailableVehiculeForLocation(location);
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

        /// <summary>
        /// permet de modifier le statut d'une location 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation([FromRoute] int id, [FromBody] LocationUpdateViewModel location)
        {
            #region Prerequisites
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid || id == 0)
            {
                return BadRequest(ModelState);
            }
            Location loc = _context.Location.FirstOrDefault(l => l.LocId == id);
            if (loc == null)
            {
                return NotFound();
            }
            #endregion

            try
            {
                LocationService locServ = new LocationService(_context);
                User user = _context.User.SingleOrDefault(u => u.UserId == loc.LocUserId);
                Vehicle vehicle = _context.Vehicle.SingleOrDefault(v => v.VehId == location.VehicleId);
                Pole poleS = _context.Pole.SingleOrDefault(p => p.PoleId == loc.LocPoleIdstart);
                Pole poleE = _context.Pole.SingleOrDefault(p => p.PoleId == loc.LocPoleIdend);

                var message1 = "";
                var message2 = "";
                bool afficherInformationVehicule = false;

                switch (location.Action)
                {
                    case "Cancel":
                        // %%MESSAGE1%% Votre location a été annulée.
                        // %%MESSAGE2%% 
                        //afficher information véhicule false
                        locServ.CancelLocation(loc);
                        message1 = "Votre location a été annulée par un administrateur...";
                        message2 = "Vous pouvez le contacter pour avoir plus de détails sur cette annulation.";
                        break;
                    case "Validate":
                        // %%MESSAGE1%% Bonne nouvelle votre location vient d'être validée !
                        // %%MESSAGE2%% Vous pouvez maintenant consulter le détail de votre location sur BookYourCar !
                        //afficher information véhicule true
                        locServ.ValidateLocationAndSetVehicule(loc, location.VehicleId);
                        message1 = "Bonne nouvelle, votre location vient d'être validée !";
                        message2 = "Vous pouvez maintenant consulter le détail de votre location sur BookYourCar !";
                        afficherInformationVehicule = true;
                        break;
                    case "Update":
                        // %%MESSAGE1%% Votre location vient d'être modifiée !
                        // %%MESSAGE2%% Vous pouvez maintenant consulter le détail de votre location sur BookYourCar !
                        //afficher information véhicule true
                        locServ.UpdateLocationAndVehicule(loc, location.VehicleId);
                        message1 = "Votre location vient d'être modifiée !";
                        message2 = "Un nouveau véhicule vous a été affecté. Consulter le détail de votre location sur BookYourCar !";
                        afficherInformationVehicule = true;
                        break;
                    case "Start":
                        // %%MESSAGE1%% Votre location vient de commencer !
                        // %%MESSAGE2%% Bonne route !
                        // afficher information véhicule true
                        locServ.StartLocation(loc);
                        message1 = "Votre location vient de commencer !";
                        message2 = "Bonne route !";
                        afficherInformationVehicule = true;
                        break;
                    case "Finish":
                        // %%MESSAGE1%% Nous esperons que votre location s'est bien passée !
                        // %%MESSAGE2%% Vous pourrez retrouver le détail de votre location sur BookYourCar !
                        // afficher information véhicule false
                        message1 = "Nous espérons que votre location s'est bien passée !";
                        message2 = "Vous pourrez retrouver le détail de votre location sur BookYourCar !";
                        locServ.FinishLocation(loc);
                        break;
                    default:
                        ModelState.AddModelError("Error", "L'action demandée est inconnue");
                        return BadRequest(ModelState);
                }
                _context.Update(loc);
                await _context.SaveChangesAsync();


                if (await EmailService.SendEmailPutLocationAsync(user, loc, poleS, poleE, vehicle, message1, message2, afficherInformationVehicule))
                {
                    ModelState.AddModelError("Success", "La location a bien été modifée.");
                    return Ok(ModelState);
                }
                else
                {
                    ModelState.AddModelError("Error",
                        "Une erreur s'est produite sur l'envoi de mail de confirmation mais la validation de la réservation a bien été prise en compte.");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Une erreur s'est produite durant la mise à jour de la location. Cause : {ex.Message}");
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
            if (TokenService.ValidateTokenWhereIsAdmin(token) && TokenService.VerifDateExpiration(token))
            {
                Location location = await _context.Location.FindAsync(id);
                if (location == null)
                {
                    return NotFound();
                }
                ////////
                location.LocState = (sbyte)Enums.LocationState.Rejected;

                AuthService service = new AuthService(_context);
                User user = service.GetUserConnected(token);
                PoleService servicePole = new PoleService(_context);
                var poleDepart = servicePole.GetPole(location.LocPoleIdstart).PoleName;
                var poleArrive = servicePole.GetPole(location.LocPoleIdend).PoleName;
                string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationRefuser);

                myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
                myFiles = myFiles.Replace("%%DEBUTLOCATION%%", location.LocDatestartlocation.ToString("dd/MM/yyyy"));
                myFiles = myFiles.Replace("%%FINLOCATION%%", location.LocDateendlocation.ToString("dd/MM/yyyy"));
                myFiles = myFiles.Replace("%%DEPARTPOLE%%", poleDepart);
                myFiles = myFiles.Replace("%%FINPOLE%%", poleArrive);
                var response = await EmailService.SendEmailAsync("Refus de votre location - BookYourCar", myFiles, user.UserEmail);
                if (response.IsSuccessStatusCode)
                {
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
            else if (TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token))
            {
                Location location = await _context.Location.FindAsync(id);
                if (location == null)
                {
                    return NotFound();
                }
                ////////
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
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();


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


                PoleService servicePole = new PoleService(_context);
                var poleDepart = servicePole.GetPole(location.LocPoleIdstart).PoleName;
                var poleArrive = servicePole.GetPole(location.LocPoleIdend).PoleName;
                string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationAsk);

                myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
                myFiles = myFiles.Replace("%%DEBUTLOCATION%%", location.LocDatestartlocation.ToString("dd/MM/yyyy"));
                myFiles = myFiles.Replace("%%FINLOCATION%%", location.LocDateendlocation.ToString("dd/MM/yyyy"));
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

        private List<AvailableVehiculeViewModel> GetAvailableVehiculeForLocation(Location location)
        {
            List<Vehicle> listAllVehicule = _context.Vehicle.ToList();

            // on ajoutera a cette liste tout les vehicules qui respecteront pas les conditions pour être valide a cette location
            List<int> listVehiculeNonDisponible = new List<int>();

            //On regarde pour chaque vehicule les locations qu'il a déja
            foreach (Vehicle vehicule in listAllVehicule)
            {
                // Gestion du cas ou le véhicule n'a pas encore de reservation il faut donc regarder le pole du vehicule directement 
                if (_context.Location.Count(x => x.LocVehId == vehicule.VehId) == 0 && vehicule.VehPoleId != location.LocPoleIdstart)
                {
                    if (!listVehiculeNonDisponible.Contains(vehicule.VehId))
                        listVehiculeNonDisponible.Add(vehicule.VehId);
                }

                //la liste des reservations du premier vehicule de la liste, puis du second etc
                List<Location> listDeslocationDuVehicule = _context.Location.Where(x => x.LocVehId == vehicule.VehId).ToList();

                //Pour chaque location de ce vehicule on va regarder les dates de ces reservations 
                foreach (Location locationDuVehicule in listDeslocationDuVehicule)
                {
                    // première condition 
                    // si il a une location qui débute avant la location en cours alors le vehicule n'est pas dispo
                    if (locationDuVehicule.LocDatestartlocation < location.LocDatestartlocation && locationDuVehicule.LocDateendlocation > location.LocDatestartlocation)
                    {
                        // si le vehicule n'est pas déja dans la liste des vehicules non dispo
                        if (!listVehiculeNonDisponible.Contains(locationDuVehicule.LocVehId.GetValueOrDefault()))
                            listVehiculeNonDisponible.Add(locationDuVehicule.LocVehId.GetValueOrDefault());
                    }

                    // 2 eme condition 
                    // On regarde ces reservations et si il en aune qui finit après la date de fin de la location en cours alors le vehicule n'est pas disponible
                    // et on l'ajoute à la liste
                    if (locationDuVehicule.LocDateendlocation > location.LocDateendlocation)
                    {
                        // si le vehicule n'est pas déja dans la liste des vehicules non dispo
                        if (!listVehiculeNonDisponible.Contains(locationDuVehicule.LocVehId.GetValueOrDefault()))
                            listVehiculeNonDisponible.Add(locationDuVehicule.LocVehId.GetValueOrDefault());
                    }

                    // 3 eme condition
                    // si la date d'une des reservations debute après la date de debut de la location en cours et que en plus la date de la reservation finit avant la date de la location
                    // alors le vehicule n'est pas disponible
                    if (locationDuVehicule.LocDatestartlocation > location.LocDatestartlocation &&
                        locationDuVehicule.LocDateendlocation < location.LocDateendlocation)
                    {
                        if (!listVehiculeNonDisponible.Contains(locationDuVehicule.LocVehId.GetValueOrDefault()))
                            listVehiculeNonDisponible.Add(locationDuVehicule.LocVehId.GetValueOrDefault());
                    }

                    // 4 eme condition
                    // si la date d'une des reservations debute avant la location en cours et finit après la date de fin de la location en cours
                    if (locationDuVehicule.LocDatestartlocation < location.LocDatestartlocation &&
                        locationDuVehicule.LocDateendlocation > location.LocDateendlocation)
                    {
                        if (!listVehiculeNonDisponible.Contains(locationDuVehicule.LocVehId.GetValueOrDefault()))
                            listVehiculeNonDisponible.Add(locationDuVehicule.LocVehId.GetValueOrDefault());
                    }

                    //todo ajouter les conditions sur les pôles
                    // 5 eme condition sur les pôles
                    // Si la date de fin des locations du vehicule finissent entre aujourd'hui et la date de debut de la location alors on regarde si son pole dernier pole d'arrive (de la voiture) correspond au pole de depart de la location
                    if (locationDuVehicule.LocDateendlocation >= DateTime.Now.ToLocalTime() &&
                        locationDuVehicule.LocDateendlocation <= location.LocDatestartlocation)
                    {
                        // on recupère la dernière location pour cette voiture pour verifier le dernier pole  d'arrive de la voiture connu 
                        Location derniereLocDuVehicule = _context.Location.Where(x => x.LocVehId == locationDuVehicule.LocVehId && x.LocDatestartlocation >= DateTime.Now)
                            .OrderBy(x => x.LocDateendlocation).FirstOrDefault();


                        //alors on regarde si son pole d'arrive correspond au pole de depart
                        if (derniereLocDuVehicule != null && derniereLocDuVehicule.LocPoleIdend != location.LocPoleIdstart)
                        {
                            listVehiculeNonDisponible.Add(locationDuVehicule.LocVehId.GetValueOrDefault());
                        }
                    }
                }
            }
            // on construit maintenant la liste des vehicules disponible en prenant tout les vehicules en base en enlevant ceux present dans la liste listVehiculeNonDisponible
            List<AvailableVehiculeViewModel> listDesVehiculeDispo = new List<AvailableVehiculeViewModel>();
            foreach (var vehicule in listAllVehicule)
            {
                if (!listVehiculeNonDisponible.Contains(vehicule.VehId))
                {

                    AvailableVehiculeViewModel vehiculeModel = new AvailableVehiculeViewModel()
                    {
                        VehId = vehicule.VehId,
                        Registration = vehicule.VehRegistration,
                        VehCommonName = vehicule.VehBrand + " " + vehicule.VehModel
                    };
                    listDesVehiculeDispo.Add(vehiculeModel);
                }
            }

            return listDesVehiculeDispo;
        }



        private VehicleDetailsViewModel GetSelectedVehicle(Location location)
        {
            if (location.LocVehId != null)
            {
                Vehicle veh = _context.Vehicle.Where(v => v.VehId == location.LocVehId).First();
                return new VehicleDetailsViewModel()
                {
                    VehId = veh.VehId,
                    VehCommonName = veh.VehBrand + " " + veh.VehModel,
                    Registration = veh.VehRegistration,
                    FuelName = veh.VehTypeEssence,
                    SeatCount = veh.VehNumberplace
                };
            }
            else
            {
                return new VehicleDetailsViewModel();
            }

        }
        #endregion
    }
}