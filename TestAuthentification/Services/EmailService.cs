using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

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
                        {"TemplateID", 1},
                        {"TemplateLanguage", true},
                        {"Subject", Titre},
                        {"HTMLPart", Contenu }
                        //{"HTMLPart", "<h3>Dear passenger 1, welcome to <a href=\"https://www.mailjet.com/\">Mailjet</a>!</h3><br />May the delivery force be with you!"}
                    }
                });

            return await client.PostAsync(request);
        }


    }
}
