using System;
using System.ComponentModel.DataAnnotations;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationViewModel
    {

        /// <summary>
        /// date de début d'une reservation
        /// </summary>
        [Required]
        public DateTime DateDebutResa { get; set; }
        /// <summary>
        /// date de fin d'une reservation
        /// </summary>
        [Required]
        public DateTime DateFinResa { get; set; }

        /// <summary>
        /// id du pôle de prise charge du véhicule
        /// </summary>
        [Required]
        public int PoleIdDepart { get; set; }
        /// <summary>
        /// id du pôle de retour du véhicule
        /// </summary>
        [Required]
        public int PoleIdDestination { get; set; }
        /// <summary>
        /// commentaire de la demande de location
        /// </summary>
        [Required]
        public string Comments { get; set; }

        
    }
}
