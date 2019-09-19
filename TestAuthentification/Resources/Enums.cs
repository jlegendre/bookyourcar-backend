using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.Resources
{
    public class Enums
    {
        /// <summary>
        /// enumeration des etats pour les locations
        /// </summary>
        public enum LocationState
        {
            Asked = 0,
            InProgress = 1,
            Validated = 2,
            // administrateur qui refuse
            Rejected = 3,
            Finished = 4,
            // utilisateur qui refuse
            Canceled = 5
        }

        public static string GetLocationStateTrad(sbyte locState)
        {
            Enums.LocationState locSt = (Enums.LocationState)locState;
            string locationStateTrad = "";
            switch (locSt)
            {
                case Enums.LocationState.Asked:
                    locationStateTrad = "Demandée";
                    break;
                case Enums.LocationState.InProgress:
                    locationStateTrad = "En cours";
                    break;
                case Enums.LocationState.Validated:
                    locationStateTrad = "Validée";
                    break;
                case Enums.LocationState.Rejected:
                    locationStateTrad = "Refusée";
                    break;
                case Enums.LocationState.Finished:
                    locationStateTrad = "Terminée";
                    break;
                case Enums.LocationState.Canceled:
                    locationStateTrad = "Annulée";
                    break;
            }
            return locationStateTrad;
        }


        public enum VehiculeState
        {
            // en utilisation
            InUse = 0,
            // valide, disponible
            Available = 1,
            // supprimé 
            Deleted = 2,
            //en maintenance
            Maintenance = 3
            
        }

        /// <summary>
        /// enumeration des roles possibles pour les utilisateurs
        /// </summary>
        public enum Roles
        {
            User,
            Admin
        }

        /// <summary>
        /// Dans l'ordre on a
        /// - InWaiting quand l'utilisateur créé son compte
        /// - EmailVerif quand l'utilisateur a vérifier son email
        /// - Validated quand l'administrateur a validé le nouveau compte
        /// - Rejected quand l'administrateur a refusé le nouveau compte
        /// - Blocked quand l'administrateur a bloqué le compte
        /// </summary>
        public enum UserState
        {
            // en attente
            InWaiting = 0,
            // compte validé par l'admin
            Validated = 1,
            // compte refusé par l'admin
            Rejected = 2,
            // compte bloqué
            Blocked = 3,
            //compte qui a été vérifié par email
            EmailVerif = 4
        }
    }
}

