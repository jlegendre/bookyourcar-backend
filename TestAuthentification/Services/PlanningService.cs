using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;
using TestAuthentification.Resources;
using TestAuthentification.ViewModels.Planning;

namespace TestAuthentification.Services
{
    public class PlanningService
    {
        public BookYourCarContext _context;

        public PlanningService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }

        internal int GetStartReservationCountThisWeek(Tuple<DateTime, DateTime> dates)
        {
            return _context.Location.Where(loc => loc.LocDatestartlocation >= dates.Item1 && loc.LocDatestartlocation <= dates.Item2).Count();
        }

        internal int GetEndReservationCountThisWeek(Tuple<DateTime, DateTime> dates)
        {
            return _context.Location.Count(loc => loc.LocDateendlocation >= dates.Item1 && loc.LocDateendlocation <= dates.Item2);
        }

        internal int GetCountTotalVehicles()
        {
            return _context.Vehicle.Count(v => v.VehState != (int)Enums.VehiculeState.Deleted);
        }

        internal int GetUsedCarToday()
        {
            return _context.Vehicle.Count(v => v.VehState == (int)Enums.VehiculeState.InUse);
        }

        internal List<VehicleReservationViewModel> GetReservationsByVehicule(Tuple<DateTime, DateTime> datesOfWeek)
        {
            DateTime startWeek = datesOfWeek.Item1;
            DateTime endWeek = datesOfWeek.Item2;

            //Get list vehicle 
            List<Vehicle> vehicles = _context.Vehicle.Where(veh => veh.VehState != (int)Enums.VehiculeState.Deleted).ToList();

            //Get locations during this week
            List<Location> locations = _context.Location.Where(loc => 
                    loc.LocDatestartlocation >= startWeek && loc.LocDatestartlocation <= endWeek 
                    || loc.LocDatestartlocation <= startWeek && loc.LocDateendlocation >= startWeek 
                    || loc.LocDateendlocation >= startWeek && loc.LocDateendlocation <= endWeek
                ).ToList();


            List<VehicleReservationViewModel> vehicleReservationsListVM = new List<VehicleReservationViewModel>();

            //Link weeklyLocation to vehicle
            foreach (Vehicle vehicle in vehicles)
            {
                VehicleReservationViewModel vehResVM = new VehicleReservationViewModel() { VehName = vehicle.VehBrand + " " + vehicle.VehModel, Immat = vehicle.VehRegistration };

                vehResVM.WeeklyReservation = locations.Where(loc => loc.LocVehId == vehicle.VehId).Select(x => new ReservationViewModel()
                {
                    //TODO   DriverName = _context.User.Select(u => u.UserId == x.LocUserId).First(),


                }).ToList();

            }





            throw new NotImplementedException();
        }
    }
}


