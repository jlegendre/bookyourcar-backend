using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;

namespace TestAuthentification.ViewModels
{
    public class UserInfoViewModel
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFirstname { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserNumpermis { get; set; }
        public int? UserRightId { get; set; }
        public int? UserPoleId { get; set; }
        public string UserPassword { get; set; }

        public Right UserPole { get; set; }
        public Right UserRight { get; set; }
        public ICollection<RideUser> RideUser { get; set; }
    }
}
