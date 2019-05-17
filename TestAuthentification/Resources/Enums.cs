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

        public enum UserState
        {
            // en attente
            InWaiting,
            // compte validé par l'admin
            Validated,
            // compte refusé par l'admin
            Rejected,
            // compte bloqué
            Blocked
        }
    }
}

