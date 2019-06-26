using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Vehicle
{
    public class VehicleDetailsViewModel
    {
        public int VehId { get; set; }
        public string VehCommonName { get; set; }
        public string Registration { get; set; }
        public int SeatCount { get; set; }
        public string FuelName { get; set; }     
        public int FuelId { get; set; }
    }
}
