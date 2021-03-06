﻿using System;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using TestAuthentification.Models;
using TestAuthentification.Resources;

namespace TestAuthentification.Services
{
    public class EmailService
    {
        private static MailjetClient client = new MailjetClient(Environment.GetEnvironmentVariable("KeyAPIEmail"), Environment.GetEnvironmentVariable("SecretAPIEmail"))
        {
            Version = ApiVersion.V3_1,
        };

        public static async Task<MailjetResponse> SendEmailAsync(string Titre, string Contenu, string EmailDestinataire)
        {
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
                .Property(Send.Messages, new JArray {
                    new JObject {
                        {"From", new JObject {
                            {"Email", "donotreply@a5d.com"},
                            {"Name", "A5D"}
                        }},
                        {"To", new JArray {
                            new JObject {
                                {"Email", EmailDestinataire},
                                {"Name", "You"}
                            }
                        }},
                        {"Subject", Titre},
                        {"HTMLPart", Contenu }
                    }
                });

            return await client.PostAsync(request);
        }

        /// <summary>
        /// Email lorsqu'on valide une reservation
        /// </summary>
        public static async Task<bool> SendEmailPutLocationAsync(Models.User user, Location loc, Pole poleS, Pole poleE, Vehicle vehicle, string message1, string message2, bool afficherInfoVehicule)
        {
            // en fonction de l'action il y a changer 
            // le titre du mail 
            // le contenu du mail si jamais la location est annulé 

            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationValidation);
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
            myFiles = myFiles.Replace("%%DEBUTLOCATION%%", loc.LocDatestartlocation.ToString("dd/MM/yyyy"));
            myFiles = myFiles.Replace("%%FINLOCATION%%", loc.LocDateendlocation.ToString("dd/MM/yyyy"));
            myFiles = myFiles.Replace("%%DEPARTPOLE%%", poleS.PoleName);
            myFiles = myFiles.Replace("%%FINPOLE%%", poleE.PoleName);

            if (afficherInfoVehicule)
            {
                myFiles = myFiles.Replace("%%AFFICHERVEHICULE%%", System.IO.File.ReadAllText(ConstantsEmail.AfficherInfoVehicule));
                myFiles = myFiles.Replace("%%MODELE%%", vehicle.VehModel);
                myFiles = myFiles.Replace("%%MARQUE%%", vehicle.VehBrand);
                myFiles = myFiles.Replace("%%IMMATRICULATION%%", vehicle.VehRegistration);
                myFiles = myFiles.Replace("%%KM%%", vehicle.VehKm.ToString());
            }
            else
            {
                myFiles = myFiles.Replace("%%AFFICHERVEHICULE%%", "");
            }
            
            myFiles = myFiles.Replace("%%MESSAGE1%%", message1);
            myFiles = myFiles.Replace("%%MESSAGE2%%", message2);



            var response = await EmailService.SendEmailAsync("Votre réservation - BookYourCar", myFiles, user.UserEmail);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
