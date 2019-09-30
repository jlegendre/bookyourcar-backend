using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using TestAuthentification.Models;
using TestAuthentification.Services.DbConfig;
using TestAuthentification.ViewModels.Location;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Resources;
using TestAuthentification.ViewModels.Vehicle;

namespace TestAuthentification.Services
{
    public class LocationService
    {
        public BookYourCarContext _context;

        public LocationService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }

        public List<AvailableVehiculeViewModel> GetAvailableVehiculeForLocation(Location location)
        {
            List<Vehicle> vehicleList = _context.Vehicle.ToList();

            List<Vehicle> preselectedVehicles = new List<Vehicle>();

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
                if (vehicle.Location == null)
                {
                    preselectedVehicles.Add(vehicle);
                }
                else
                {
                    if (vehicle.Location.Where(l => l.LocDatestartlocation <= location.LocDatestartlocation && l.LocDateendlocation >= location.LocDateendlocation).Count() == 0)
                    {
                        if (vehicle.Location.Where(l => l.LocDateendlocation <= location.LocDatestartlocation && l.LocDateendlocation >= location.LocDatestartlocation).Count() == 0)
                        {
                            if (vehicle.Location.Where(l => l.LocDateendlocation <= location.LocDatestartlocation && l.LocDateendlocation >= location.LocDatestartlocation).Count() == 0)
                            {
                                if (vehicle.Location.Where(l => l.LocDatestartlocation >= location.LocDatestartlocation && l.LocDatestartlocation <= location.LocDateendlocation).Count() == 0)
                                {
                                    preselectedVehicles.Add(vehicle);
                                }
                            }
                        }
                    }
                }
            }
            List<Vehicle> selectedVehicles = new List<Vehicle>();
            if (preselectedVehicles.Count > 0)
            {
                Location lastLoc, nextLoc = new Location();
                foreach (Vehicle vehicle in preselectedVehicles)
                {
                    List<Location> locs = vehicle.Location.Where(l => l.LocDateendlocation < location.LocDatestartlocation).ToList();
                    if (locs.Count == 0)
                    {
                        if (vehicle.VehPoleId == location.LocPoleIdstart)
                        {
                            selectedVehicles.Add(vehicle);
                        }
                    }
                    else
                    {
                        lastLoc = locs.Where(l => l.LocDateendlocation < location.LocDatestartlocation).OrderByDescending(l => l.LocDateendlocation).First();
                        nextLoc = locs.Where(l => l.LocDatestartlocation < location.LocDateendlocation).OrderBy(l => l.LocDatestartlocation).First();

                        if (lastLoc.LocPoleIdend != null)
                        {
                            if (nextLoc.LocPoleIdstart == location.LocPoleIdend)
                            {
                                selectedVehicles.Add(vehicle);
                                continue;
                            }
                        }
                        if (nextLoc.LocPoleIdend != null)
                        {
                            if (lastLoc.LocPoleIdend == location.LocPoleIdstart)
                            {
                                selectedVehicles.Add(vehicle);
                                continue;
                            }
                        }
                        if (lastLoc.LocPoleIdend == location.LocPoleIdstart && nextLoc.LocPoleIdstart == location.LocPoleIdend)
                        {
                            selectedVehicles.Add(vehicle);
                            continue;
                        }
                    }
                }
            }
            List<AvailableVehiculeViewModel> availableVehicules = new List<AvailableVehiculeViewModel>();
            //construct viewModels
            foreach (Vehicle vehicle in selectedVehicles)
            {
                AvailableVehiculeViewModel available = new AvailableVehiculeViewModel()
                {
                    VehId = vehicle.VehId,
                    Registration = vehicle.VehRegistration,
                    VehCommonName = vehicle.VehBrand + " - " + vehicle.VehModel
                };
                availableVehicules.Add(available);
            }
            return availableVehicules;
        }

        public async Task<List<LocationListViewModel>> GetAllLocationAsync()
        {
            List<Location> listLocation = await _context.Location.ToListAsync();

            List<LocationListViewModel> locations = new List<LocationListViewModel>();

            if (listLocation.Count > 0)
            {
                foreach (Location loc in listLocation)
                {
                    LocationListViewModel locVM = new LocationListViewModel();
                    locVM.LocationId = loc.LocId;
                    locVM.DateDebutResa = loc.LocDatestartlocation.ToString("d");
                    locVM.DateFinResa = loc.LocDateendlocation.ToString("d");

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
            return locations.OrderByDescending(x=>x.LocationState).ToList();
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

        internal void ValidateLocationAndSetVehicule(Location loc, int vehicleId)
        {
            if (loc.LocState == (sbyte)Enums.LocationState.Asked)
            {
                loc.LocState = (int)Enums.LocationState.Validated;
                loc.LocVehId = vehicleId;
            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet pas cette action"));
            }
        }

        internal void UpdateLocationAndVehicule(Location loc, int vehicleId)
        {
            if (loc.LocState == (sbyte)Enums.LocationState.Validated)
            {
                loc.LocState = (int)Enums.LocationState.Validated;
                loc.LocVehId = vehicleId;
            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet cette action"));
            }
        }

        internal void CancelLocation(Location loc)
        {
            if (loc.LocState == (sbyte)Enums.LocationState.Asked || loc.LocState == (sbyte)Enums.LocationState.Validated)
            {
                loc.LocState = (int)Enums.LocationState.Rejected;
                loc.LocVehId = null;
            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet cette action"));
            }
        }

        internal void StartLocation(Location loc)
        {
            if (loc.LocState == (sbyte)Enums.LocationState.Validated)
            {
                loc.LocState = (int)Enums.LocationState.InProgress;
                // changer statut du vehicule
                try
                {
                    Vehicle vehicleAboutLocation = _context.Vehicle.FirstOrDefault(x => x.VehId == loc.LocVehId);
                    if (vehicleAboutLocation != null)
                    {
                        vehicleAboutLocation.VehState = (sbyte)Enums.VehiculeState.InUse;
                        _context.Vehicle.Update(vehicleAboutLocation);
                    }
                }
                catch (Exception e)
                {
                    throw (new Exception(message: "Une erreur s'est produite sur la location. " + e.Message));
                }
            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet cette action"));
            }
        }

        internal void FinishLocation(Location loc)
        {
            if (loc.LocState == (sbyte)Enums.LocationState.InProgress)
            {
                loc.LocState = (int)Enums.LocationState.Finished;

                // mettre le pole de fin de location à la voiture de la location
                try
                {
                    Vehicle vehicleAboutLocation = _context.Vehicle.FirstOrDefault(x => x.VehId == loc.LocVehId);
                    if (vehicleAboutLocation != null)
                    {
                        vehicleAboutLocation.VehPoleId = loc.LocPoleIdend;
                        vehicleAboutLocation.VehState = (sbyte)Enums.VehiculeState.Available;
                        _context.Vehicle.Update(vehicleAboutLocation);
                    }
                }
                catch (Exception e)
                {
                    throw (new Exception(message: "Une erreur s'est produite sur la location. " + e.Message));
                }
            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet cette action"));
            }
        }
    }
}


