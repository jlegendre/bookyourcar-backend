using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Planning
{
    public class PlanningViewModel
    {
        public DateTime StartWeek { get; set; }
        public DateTime EndWeek { get; set; }
        public int StartReservationCount { get; set; }
        public int EndReservationCount { get; set; }
        public int TotalVehiclesCount { get; set; }
        public int UsedVehiclesCount { get; set; }
        public List<VehicleReservationViewModel> ListOfReservationsByVehicule { get; internal set; }
    }
}
