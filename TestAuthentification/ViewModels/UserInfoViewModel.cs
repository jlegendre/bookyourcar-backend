using System.ComponentModel.DataAnnotations;

namespace TestAuthentification.ViewModels
{
    public class UserInfoViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string UserFirstname { get; set; }

        [Display(Name = "Téléphone")]
        public string UserPhone { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Numéro de permis")]
        public string UserNumpermis { get; set; }

        public int? UserRightId { get; set; }

        public int? UserPoleId { get; set; }

        [Display(Name = "Pôle")]
        public string PoleName { get; set; }
    }
}
