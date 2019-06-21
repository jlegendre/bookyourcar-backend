using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mailjet.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
                    expires: loginViewModel.RememberMe ? DateTime.UtcNow.AddDays(5) : DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signinCredentials
                );

                string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
                //return CreatedAtAction(nameof(GetUserInfo), new { Token = tokenString });
            }
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

            IdentityResult result = _authService.VerifUser(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", result.Errors.First().Description);

                return BadRequest(ModelState);
            }

            IdentityResult result2 = _authService.AddToRoleUserAsync(user);
            if (!result2.Succeeded)
            {
                ModelState.AddModelError("Error", result2.Errors.First().Description);
                return BadRequest(ModelState);
            }

            IdentityResult result3 = _authService.VerifPhoneNumber(user);
            if (!result3.Succeeded)
            {
                ModelState.AddModelError("Error", result3.Errors.First().Description);
                return BadRequest(ModelState);
            }


            // mise à jour de l'état du compte
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

#if !DEBUG
            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.RegisterPath);
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
            var response = await EmailService.SendEmailAsync("Création d'un nouveau compte - BookYourCar", myFiles, user.UserEmail);
            if (!response.IsSuccessStatusCode)
            {
                                ModelState.AddModelError("Error",
                    "Une erreur s'est produite sur l'envoi de mail de confirmation mais la validation de la réservation a bien été prise en compte.");
                return BadRequest(ModelState);
            }
#endif
            return Ok();

        }

        /// <summary>
        /// fonction -->  valider formulaire avec l'email suite à l'action mot de passe oublié
        /// </summary>
        /// <param name="emailDestinataire"></param>
        /// <returns></returns>
        [HttpPost, Route("PasswordForget")]
        public async Task<IActionResult> PasswordForgetAsync([FromBody] string emailDestinataire)
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
                new Claim(ClaimTypes.Role, user.UserRight.RightLabel)
            };

            // On Définit les proprietées du token, comme ça date d'expiration
            JwtSecurityToken tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: signinCredentials
            );

            string tokenGenerate = new JwtSecurityTokenHandler().WriteToken(tokeOptions);


            string myFiles = System.IO.File.ReadAllText(ConstantsEmail.ResetPassword);
            myFiles = myFiles.Replace("%%TOKEN%%", tokenGenerate);
            var response = await EmailService.SendEmailAsync("Changement de mot de passe - BookYourCar", myFiles, user.UserEmail);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] string token)
        {
            if (TokenService.ValidateToken(token))
            {
                var message = new Dictionary<string, string>();
                message.Add("Info", "OK LIEN");
                return Ok(message);
            }

            return Unauthorized();

        }







    }
}