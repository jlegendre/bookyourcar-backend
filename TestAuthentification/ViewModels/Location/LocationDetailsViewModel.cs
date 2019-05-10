using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationDetailsViewModel
    {
        /// <summary>
        /// Utilisateur associé à la location
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// id du véhicule choisis pour une reservation
        /// </summary>
        public string LocationState { get; set; }

        /// <summary>
        /// date de début d'une reservation
        /// </summary>
        public DateTime DateDebutResa { get; set; }

        /// <summary>
        /// date de fin d'une reservation
        /// </summary>
        public DateTime DateFinResa { get; set; }

        /// <summary>
        /// id du pôle de prise charge du véhicule
        /// </summary>
        public int PoleIdDepart { get; set; }

        /// <summary>
        /// id du pôle de retour du véhicule
        /// </summary>
        public int PoleIdDestination { get; set; }

        /// <summary>
        /// vehicule attirbué pour la location
        /// </summary>
        public Vehicle SelectedVehicle { get; set; }

        /// <summary>
        /// liste de vehicules pouvant être attribués à la location
        /// </summary>
        public List<Vehicle> AvailableVehicle { get; set; }

    }
}
