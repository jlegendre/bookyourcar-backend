using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            Comments = new HashSet<Comments>();
            Historymaintenance = new HashSet<Historymaintenance>();
            Images = new HashSet<Images>();
            Key = new HashSet<Key>();
            Location = new HashSet<Location>();
        }

        public int VehId { get; set; }
        public string VehRegistration { get; set; }
        public string VehBrand { get; set; }
        public string VehModel { get; set; }
        public float VehKm { get; set; }
        public DateTime VehDatemec { get; set; }
        public string VehTypeEssence { get; set; }
        public string VehColor { get; set; }
        public bool VehIsactive { get; set; }
        public sbyte VehState { get; set; }
        public int VehNumberplace { get; set; }
        public int? VehPoleId { get; set; }

        public Pole VehPole { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Historymaintenance> Historymaintenance { get; set; }
        public ICollection<Images> Images { get; set; }
        public ICollection<Key> Key { get; set; }
        public ICollection<Location> Location { get; set; }
    }
}
