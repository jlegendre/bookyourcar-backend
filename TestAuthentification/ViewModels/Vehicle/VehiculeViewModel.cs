using System;
using System.ComponentModel.DataAnnotations;

namespace TestAuthentification.ViewModels
{
    public class VehiculeViewModel
    {
        [Key]
        public int VehId { get; set; }

        [Required]
        [Display(Name = "Immatriculation")]
        public string VehRegistration { get; set; }

        [Required]
        [Display(Name = "Marque")]
        public string VehBrand { get; set; }

        [Required]
        [Display(Name = "Modele")]
        public string VehModel { get; set; }

        [Display(Name = "Nombre de km")]
        public float VehKm { get; set; }

        [Display(Name = "Date de mise en circulation")]
        public DateTime VehDatemec { get; set; }

        [Display(Name = "Type Essence")]
        public string VehTypeEssence { get; set; }

        [Display(Name = "Couleur")]
        public string VehColor { get; set; }

        [Display(Name = "Nombre de places")]
        public int VehNumberplace { get; set; }

        public bool VehIsactive { get; set; }

        public int? PoleId { get; set; }

        public string PoleName { get; set; }
    }
}
