using System;
using System.Collections.Generic;

namespace TestAuthentification.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comments>();
            Location = new HashSet<Location>();
            RideUser = new HashSet<RideUser>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFirstname { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserNumpermis { get; set; }
        public int? UserRightId { get; set; }
        public int? UserPoleId { get; set; }
        public string UserPassword { get; set; }
        public bool UserIsactivated { get; set; }
        public sbyte UserState { get; set; }

        public Pole UserPole { get; set; }
        public Right UserRight { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Location> Location { get; set; }
        public ICollection<RideUser> RideUser { get; set; }
    }
}
