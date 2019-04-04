using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestAuthentification.Models;

namespace TestAuthentification.ViewModels.Vehicle
{
    public class ListVehiculeViewModel
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
        [Display(Name = "Carburant")]
        public string VehTypeEssence { get; set; }
        [Display(Name = "Couleur")]
        public string VehColor { get; set; }
        [Display(Name = "Nombre de places")]
        public int VehNumberplace { get; set; }

        public string Pole { get; set; }
        
    }
}
