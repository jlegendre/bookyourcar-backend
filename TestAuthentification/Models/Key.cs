using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Key
    {
        public int KeyId { get; set; }
        public string KeyLocalisation { get; set; }
        public sbyte KeyAvailable { get; set; }
        public string KeyStatus { get; set; }
        public int KeyCarId { get; set; }

        public Vehicle KeyCar { get; set; }
    }
}
