using System;
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
        public static async Task<bool> SendEmailPutLocationAsync(Models.User user, Location loc, Pole poleS, Pole poleE, Vehicle vehicle, string action)
        {
            //TODO : Send specific mail on action 
            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.LocationValidation);
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
            myFiles = myFiles.Replace("%%DEBUTLOCATION%%", loc.LocDatestartlocation.ToLongDateString());
            myFiles = myFiles.Replace("%%FINLOCATION%%", loc.LocDateendlocation.ToLongDateString());
            myFiles = myFiles.Replace("%%DEPARTPOLE%%", poleS.PoleName);
            myFiles = myFiles.Replace("%%FINPOLE%%", poleE.PoleName);

            myFiles = myFiles.Replace("%%MODELE%%", vehicle.VehModel);
            myFiles = myFiles.Replace("%%MARQUE%%", vehicle.VehBrand);
            myFiles = myFiles.Replace("%%IMMATRICULATION%%", vehicle.VehRegistration);
            myFiles = myFiles.Replace("%%KM%%", vehicle.VehKm.ToString());

            var response = await EmailService.SendEmailAsync("Validation de votre réservation - BookYourCar", myFiles, user.UserEmail);
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
