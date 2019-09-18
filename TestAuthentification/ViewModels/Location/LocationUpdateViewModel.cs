using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationUpdateViewModel
    {

        public int VehicleId { get; set; }

        /// <summary>
        /// Action réalisé côté front
        /// Soit : 
        ///     Validate
        ///     Finish
        ///     Start
        /// </summary>
        public string Action { get; set; }
    }
}
