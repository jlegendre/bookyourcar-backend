using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.ViewModels.Vehicle;

namespace TestAuthentification.ViewModels.Location
{
    public class ManageLocationViewModel
    {
        public int LocId { get; set; }
        public int LocStateId { get; set; }
        public string LocState { get; set; }
        public string User { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string PoleStart { get; set; }
        public string PoleEnd { get; set; }
        public string Comment { get; set; }

        public VehicleDetailsViewModel SelectedVehicle { get; set; }
        public List<AvailableVehiculeViewModel> AvailableVehicles { get; set; }
    }
}
