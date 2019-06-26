using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Planning
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string DriverName { get; set; }

    }
}
