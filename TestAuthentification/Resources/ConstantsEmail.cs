using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuthentification.Resources
{
    public class ConstantsEmail
    {
        #region Register

        /// <summary>
        /// envoie d'email quand on créer un compte avec un recap des informations 
        /// </summary>
        public const string Register = "Bonjour, <br />" +
                                       "Vous venez de créer un nouveau compte sur Book Your Car. <br />" +
                                       "Vous devez maintenant attendre la validation par l'administrateur du site. <br />" +
                                       "Vous serez tenues informées sur le statut de votre demande lors de sa réponse. <br /><br />" +
                                       "Ceci est un mail automatique merci de ne pas y répondre." +
                                       "Cordialement, <br />" +
                                       "L'Équipe A5D - Book Your Car ";
        /// <summary>
        /// message validation KO (refus) de création de compte
        /// </summary>
        public const string RefusRegister = "Votre demande de création de compte n'a malheureusement pas été accepté par notre Equipe." +
                                            "Cordialement, <br />" +
                                            "L'Équipe A5D - Book Your Car ";
        /// <summary>
        /// message validation OK création de compte
        /// </summary>
        public const string ValidateRegister = "Bonjour, <br />" +
                                               "Votre demande de création de compte vient d'être validé par l'Administrateur. <br />" +
                                               "Vous pouvez maintenant accéder au logiciel Book Your Car avec vos identifiants. <br />" +
                                               "Cordialement, <br />" +
                                               "L'Équipe A5D - Book Your Car ";

        #endregion

        #region Login

        public static readonly string LoginResetPassword = "Voici le lien pour rénitialiser votre mot de passe </br>" +
                                                           Environment.GetEnvironmentVariable("UrlResetPassword");

        #endregion


        /// <summary>
        /// envoie d'un email lorsqu'un utilisateur fait une nouvelle réservation
        /// </summary>
        public const string ValidationReservation = "";

    }
}
