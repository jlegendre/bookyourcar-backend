using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationListViewModel
    {
        /// <summary>
        /// id du véhicule choisis pour une reservation
        /// </summary>
        public int? VehId { get; set; }

        /// <summary>
        /// id de l'utilisateur choisis pour une reservation
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// id du véhicule choisis pour une reservation
        /// </summary>
        public int UserName { get; set; }

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
    }
}
