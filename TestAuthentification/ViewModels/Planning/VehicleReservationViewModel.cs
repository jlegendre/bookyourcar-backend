using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Planning
{
    public class VehicleReservationViewModel
    {
        public int VehId { get; set; }
        public string VehName { get; set; }
        [DisplayName("Immatriculation")]
        public string Immat { get; set; }
        public List<ReservationViewModel> WeeklyReservation { get; set; }
    }
}
