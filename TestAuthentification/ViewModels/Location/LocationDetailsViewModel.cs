﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;
using TestAuthentification.ViewModels.Comments;

namespace TestAuthentification.ViewModels.Location
{
    public class LocationDetailsViewModel
    {
        /// <summary>
        /// Utilisateur associé à la location
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Etat de la location
        /// </summary>
        public string LocationState { get; set; }

        /// <summary>
        /// Etat de la location
        /// </summary>
        public sbyte LocationStateId { get; set; }

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
        public string PoleDepart { get; set; }

        /// <summary>
        /// id du pôle de retour du véhicule
        /// </summary>
        public string PoleDestination { get; set; }

        /// <summary>
        /// liste de commentaires pour la location
        /// </summary>
        public List<CommentsViewModel> CommentsList { get; set; }

        /// <summary>
        /// vehicule attribué pour la location
        /// </summary>
        public VehiculeViewModel SelectedVehicle { get; set; }

        /// <summary>
        /// liste de vehicules pouvant être attribués à la location
        /// </summary>
        public List<VehiculeViewModel> AvailableVehicle { get; set; }

    }
}
