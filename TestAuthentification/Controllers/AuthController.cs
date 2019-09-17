using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using TestAuthentification.Models;
using TestAuthentification.Resources;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static BookYourCarContext _context;
        private readonly AuthService _authService;
        private PasswordHasherService<User> service = new PasswordHasherService<User>();

        public AuthController(BookYourCarContext context)
        {
            _context = context;
            _authService = new AuthService(_context);

        }


        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]LoginViewModel loginViewModel)
        {
            if (loginViewModel == null && !ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            // On recupère l'utilisateur en fonction de son email
            User myUser = _authService.FindByEmail(loginViewModel.Email);

            // On regarde si le password correspond avec celui du formulaire 
            // si c'est le cas on créé un jeton d'authentification Token
            if (myUser != null && AuthService.CheckPassword(myUser, myUser.UserPassword, loginViewModel.Password) && myUser.UserState.Equals((sbyte)Enums.UserState.Validated))
            {
                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey"));
                SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                Claim[] claims = new[]
               {
                    new Claim(ClaimTypes.Email, myUser.UserEmail),
                    new Claim(ClaimTypes.Role, myUser.UserRight.RightLabel)
                };

                // On Définit les proprietées du token, comme ça date d'expiration
                JwtSecurityToken tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: claims,
                    expires: loginViewModel.RememberMe ? DateTime.UtcNow.AddDays(5).ToLocalTime() : DateTime.UtcNow.AddMinutes(10).ToLocalTime(),
                    signingCredentials: signinCredentials
                );

                string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
                //return CreatedAtAction(nameof(GetUserInfo), new { Token = tokenString });
            }
            // si on est ici c'est que le compte n'est pas encore activé 
            else if (myUser != null && !myUser.UserState.Equals((sbyte)Enums.UserState.Validated))
            {
                ModelState.AddModelError("Error", "Votre compte n'est pas encore activé");
                return BadRequest(ModelState);
            }
            else
            {
                ModelState.AddModelError("Error", "Mot de passe ou Email invalide.");
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Création d'un compte 
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        // POST api/Account/Register
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = new User()
            {
                UserPassword = service.HashPassword(null, registerViewModel.Password),
                UserEmail = registerViewModel.Email,
                UserFirstname = registerViewModel.Prenom,
                UserName = registerViewModel.Nom,
                UserPoleId = registerViewModel.PoleId,
                UserPhone = registerViewModel.PhoneNumber,
                UserNumpermis = registerViewModel.NumPermis
            };

            //Vérification de l'email
            IdentityResult result = _authService.VerifUser(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", result.Errors.First().Description);

                return BadRequest(ModelState);
            }

            // Ajout du rôle à l'utilisateur 
            IdentityResult result2 = _authService.AddToRoleUserAsync(user);
            if (!result2.Succeeded)
            {
                ModelState.AddModelError("Error", result2.Errors.First().Description);
                return BadRequest(ModelState);
            }

            // verification de l'unicité du téléphone
            IdentityResult result3 = _authService.VerifPhoneNumber(user);
            if (!result3.Succeeded)
            {
                ModelState.AddModelError("Error", result3.Errors.First().Description);
                return BadRequest(ModelState);
            }


            // mise à jour de l'état du compte à en attente 
            user.UserState = (sbyte)Enums.UserState.InWaiting;

            try
            {
                _context.User.Add(user);
                _context.SaveChanges();
                _context.Dispose();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", "Une erreur est survenue.");
                Console.WriteLine(e);
                return BadRequest(ModelState);
            }

            // token valide 10 mins
            var tokenGenerate = TokenService.GenerateToken(user);

            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.RegisterPath);
            myFiles = myFiles.Replace("%%LIEN%%", Environment.GetEnvironmentVariable("UrlVerifEmail") + tokenGenerate);
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);

            try
            {
                var response =
                    await EmailService.SendEmailAsync("confirmez votre e-mail - BookYourCar", myFiles, user.UserEmail);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite sur l'envoi de mail.");
                return BadRequest(ModelState);
            }

            return Ok();
        }




        [HttpGet("VerifEmail/{token}")]
        public async Task<IActionResult> VerifEmail(string token)
        {
            var message = new Dictionary<string, string>();

            if (TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token))
            {
                //si l'utilisateur a un statut différent de InWaiting alors on valide ça verification de compte sinon on informe l'utilisateur qu'il a déja verifié son compte
                try
                {
                    User user = _authService.GetUserConnected(token);
                    if (user.UserState != (sbyte)Enums.UserState.InWaiting)
                    {
                        user.UserState = (sbyte)Enums.UserState.EmailVerif;
                        _context.User.Update(user);
                        _context.SaveChanges();

                        message.Add("Info", "Merci ! L'adresse mail vient d'être confirmé. Vous pouvez fermer l'onglet.");
                        return Ok(message);
                    }
                    else
                    {
                        message.Add("Info", "L'adresse mail a déja été verifié. Vous pouvez fermer l'onglet.");
                        return Ok(message);
                    }

                }
                catch (Exception e)
                {
                    if (e.Source != null)
                        Console.WriteLine("IOException source: {0}", e.Source);
                    throw;
                }
            }
            else
            {
                return Unauthorized();
            }
        }



        /// <summary>
        /// fonction -->  valider formulaire avec l'email suite à l'action mot de passe oublié
        /// </summary>
        /// <param name="emailDestinataire"></param>
        /// <returns></returns>
        [HttpPost, Route("PasswordForget")]
        public async Task<IActionResult> PasswordForgetAsync(string emailDestinataire)
        {
            AuthService serviceAuth = new AuthService(_context);
            if (!serviceAuth.CheckEmail(emailDestinataire) || !ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "L'email saisie ne correspond à aucun compte.");
                return BadRequest(ModelState);
            }

            var user = serviceAuth.FindByEmail(emailDestinataire);

            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey"));
            SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRight.RightLabel),
            };

            // On Définit les proprietées du token, comme ça date d'expiration


            JwtSecurityToken tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(1).ToLocalTime(),
                signingCredentials: signinCredentials
            );

            string tokenGenerate = new JwtSecurityTokenHandler().WriteToken(tokeOptions);


            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.ResetPassword);
            myFiles = myFiles.Replace("%%LIEN%%", Environment.GetEnvironmentVariable("UrlResetPassword") + tokenGenerate);
            var response = await EmailService.SendEmailAsync("Réinitialisation du mot de passe - BookYourCar", myFiles, user.UserEmail);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Error",
                    "Une erreur s'est produite sur l'envoi de mail de confirmation mais la validation de la réservation a bien été prise en compte.");
                return BadRequest(ModelState);
            }

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());

                var message = new Dictionary<string, string>();
                message.Add("Info", "Un email de rénitialisation vient de vous être envoyé.");
                return Ok(message);
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                ModelState.AddModelError("Error", "Une erreur est survenue.");
                return BadRequest(ModelState);
            }

        }
        /// <summary>
        /// fonction permettant de rediriger l'utilisateur(depuis le lien présent dans l'email) vers un page de rénitialisation de mot passe avec
        /// nouveau mot de passe
        /// confirm nouveau mot de passe
        /// Ce lien de redirection contient en paramètre un token valide pendant 5 mins
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("ChangePassword/{token}")]
        public async Task<IActionResult> ChangePassword(string token)
        {
            if (TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token))
            {
                // on vérifie si la date d'expiration du token est valide

                var message = new Dictionary<string, string>();
                message.Add("Token", token);

                // on retourne la vue du formulaire pour reset les infos
                return Ok(message);
            }
            else
            {
                return Unauthorized();
            }
        }





        /// <summary>
        /// Permet de valider le nouveau mot de passe que l'utilisateur à saisit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveChangePassword")]
        public async Task<IActionResult> SaveChangePassword(ResetPasswordViewModel model)
        {
            var token = Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer"))
            {
                var tab = token.Split(" ");
                token = tab[1];
            }

            if (TokenService.ValidateToken(token) && TokenService.VerifDateExpiration(token))
            {
                AuthService serviceAuth = new AuthService(_context);
                var userConnected = serviceAuth.GetUserConnected(token);

                userConnected.UserPassword = service.HashPassword(null, model.Password);

                //SAVE
                _context.User.Update(userConnected);
                _context.SaveChanges();

                return Ok();
            }

            var message = new Dictionary<string, string>();
            message.Add("Info", "Le token a expiré. Veuillez recommencer la procédure de rénitialisation.");

            return Ok(message);

        }

    }
}