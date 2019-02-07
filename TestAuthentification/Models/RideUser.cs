using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class RideUser
    {
        public int RideId { get; set; }
        public int UserId { get; set; }

        public Ride Ride { get; set; }
        public User User { get; set; }
    }
}
