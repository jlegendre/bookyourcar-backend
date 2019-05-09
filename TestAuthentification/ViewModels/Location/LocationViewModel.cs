

using System;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationViewModel
    {

        /// <summary>
        /// id du véhicule choisis pour une reservation
        /// </summary>
        public int VehId { get; set; }
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

    }
}
