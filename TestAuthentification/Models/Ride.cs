using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Ride
    {
        public Ride()
        {
            RideUser = new HashSet<RideUser>();
        }

        public int RideId { get; set; }
        public DateTime RideHourstart { get; set; }
        public int RideLocId { get; set; }
        public int RidePoleIdstart { get; set; }
        public int RidePoleIdend { get; set; }

        public Location RideLoc { get; set; }
        public Pole RidePoleIdendNavigation { get; set; }
        public Pole RidePoleIdstartNavigation { get; set; }
        public ICollection<RideUser> RideUser { get; set; }
    }
}
