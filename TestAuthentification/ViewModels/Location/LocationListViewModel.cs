﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationListViewModel
    {
        /// <summary>
        /// id de la location
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        /// id du véhicule choisis pour une reservation
        /// </summary>
        public string VehicleFriendlyName { get; set; }

        /// <summary>
        /// user pour une reservation
        /// </summary>
        public string UserFriendlyName { get; set; }

        /// <summary>
        /// statut de la reservation
        /// </summary>
        public string LocationState { get; set; }

        /// <summary>
        /// id statut de la reservation
        /// </summary>
        public sbyte LocationStateId { get; set; }

        /// <summary>
        /// date de début d'une reservation
        /// </summary>
        public string DateDebutResa { get; set; }

        /// <summary>
        /// date de fin d'une reservation
        /// </summary>
        public string DateFinResa { get; set; }

        /// <summary>
        /// id du pôle de prise charge du véhicule
        /// </summary>
        public string PoleDepart { get; set; }

        /// <summary>
        /// id du pôle de retour du véhicule
        /// </summary>
        public string PoleDestination { get; set; }
    }
}
