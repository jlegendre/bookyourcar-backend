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
            Rejected = 3,
            Finished = 4,
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
    }
}

