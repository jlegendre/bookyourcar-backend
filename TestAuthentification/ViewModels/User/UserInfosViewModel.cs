using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.User
{
    public class UserInfosViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Pole { get; set; }

        public string Right { get; set; }

        public string DrivingLicence { get; set; }

        public int LocationsCount { get; set; }

        public DateTime? NextLocation { get; set; }

        public int? NextLocationId { get; set; }

        public string UrlProfileImage { get; set; }
    }
}
