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

namespace TestAuthentification.Services
{
    public class LocationService
    {
        public BookYourCarContext _context;

        public LocationService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }

        public List<Vehicle> GetAvailableVehicleForLocation(DateTime dateDebut, DateTime dateFin, int? poleDebut, int? poleFin)
        {
            try
            {
                List<MySqlParameter> parametres = new List<MySqlParameter>
                {
                    new MySqlParameter(
                        "DATEDEBUT", MySqlDbType.DateTime, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, dateDebut),
                    new MySqlParameter(
                        "DATEFIN", MySqlDbType.DateTime, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, dateFin),
                    new MySqlParameter(
                        "POLESTART", MySqlDbType.Int32, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, poleDebut),
                    new MySqlParameter(
                        "POLEEND", MySqlDbType.Int32, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, poleFin)
                };
                using (var dbm = new DbManager())
                {
                    return dbm.ExecuteList<Vehicle>("getAvailableVehicle", parametres);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    throw (new Exception(message: "Une erreur c'est produite sur la location. " + e.Message));
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
                    throw (new Exception(message: "Une erreur c'est produite sur la location. " + e.Message));
                }



            }
            else
            {
                throw (new Exception(message: "le statut de location ne permet cette action"));
            }
        }

        /**using (var connection = new MySqlConnection("server=mvinet.fr;port=3306;database=BookYourCar;uid=a5d;password=pwtk@[gh$!7Z#&wX"))
{
   var command = new MySqlCommand("getAvailableVehicle", connection);
   command.CommandType = CommandType.StoredProcedure;
   command.Parameters.Add(new MySqlParameter("DATEDEBUT", dateDebut));
   command.Parameters.Add(new MySqlParameter("DATEFIN", dateFin));
   command.Parameters.Add(new MySqlParameter("POLESTART", poleDebut));
   command.Parameters.Add(new MySqlParameter("POLEEND", poleFin));
   command.Connection.Open();
   var result = command.ExecuteReader();
   command.Connection.Close();

   var listeVehicule = new List<Vehicle>();
   foreach (var vehicule in result)
   {
       var veh = new Vehicle();
       while (result.HasRows)
       {
           //TODO recuperer les valeurs et les mettres dans un objet Vehicule
       }

   }

}**/

        //_context.Database.ExecuteSqlCommand("getAvailableVehicle @p0, @p1, @p2, @p3", parameters: new[] { DateTime.Now.ToString(), DateTime.Now.ToString(), 1.ToString(), 1.ToString() });

    }
}


