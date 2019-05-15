using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Historymaintenance
    {
        public int HistId { get; set; }
        public string HistCitygarage { get; set; }
        public string HistCpgarage { get; set; }
        public DateTime? HistDateendmaintenance { get; set; }
        public DateTime? HistDatestartmaintenance { get; set; }
        public string HistReffacture { get; set; }
        public int? HistVehId { get; set; }

        public Vehicle HistVeh { get; set; }
    }
}
