using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class Right
    {
        public Right()
        {
            User = new HashSet<User>();
        }

        public int RightId { get; set; }
        public string RightLabel { get; set; }

        public ICollection<User> User { get; set; }
    }
}
