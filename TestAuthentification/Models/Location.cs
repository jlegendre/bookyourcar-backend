using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Location
    {
        public Location()
        {
            Ride = new HashSet<Ride>();
        }

        public int LocId { get; set; }
        public DateTime LocDatestartlocation { get; set; }
        public DateTime LocDateendlocation { get; set; }
        public int LocVehId { get; set; }
        public int LocPoleIdstart { get; set; }
        public int LocPoleIdend { get; set; }

        public Pole LocPoleIdendNavigation { get; set; }
        public Pole LocPoleIdstartNavigation { get; set; }
        public Vehicle LocVeh { get; set; }
        public ICollection<Ride> Ride { get; set; }
    }
}
