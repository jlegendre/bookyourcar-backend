using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.User
{
    public class UserInfosViewModel
    {
        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }
        
        [Display(Name = "Téléphone")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Pôle")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Pôle")]
        public string Pole { get; set; }

        public string Right { get; set; }

        public string DrivingLicence { get; set; }

        public int LocationsCount { get; set; }

        public DateTime? NextLocation { get; set; }

        public int? NextLocationId { get; set; }

        public string UrlProfileImage { get; set; }
    }
}
