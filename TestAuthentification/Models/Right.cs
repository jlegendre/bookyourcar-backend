using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Right
    {
        public Right()
        {
            UserUserPole = new HashSet<User>();
            UserUserRight = new HashSet<User>();
        }

        public int RightId { get; set; }
        public string RightLabel { get; set; }

        public ICollection<User> UserUserPole { get; set; }
        public ICollection<User> UserUserRight { get; set; }
    }
}
