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
                UserNumpermis = registerViewModel.NumPermis,
                UserRightId = 1  //Utilisateur
            };

            IdentityResult result = _authService.VerifUser(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            IdentityResult result2 = _authService.AddToRoleUserAsync(user);
            if (!result2.Succeeded)
            {
                return BadRequest(result2.Errors);
            }

            IdentityResult result3 = _authService.VerifPhoneNumber(user);
            if (!result3.Succeeded)
            {
                return BadRequest(result3.Errors);
            }


            // mise à jour de l'état du compte
            user.UserState = (sbyte)Enums.UserState.InWaiting;


            _context.User.Add(user);
            _context.SaveChanges();
            _context.Dispose();

            //await EmailService.SendEmailAsync("Création d'un nouveau compte - Book Your Car", String.Format(myFiles, user.UserFirstname), user.UserEmail);

            string myFiles = System.IO.File.ReadAllText("./wwwroot/EmailTemplates/Register.html");
            //myFiles.Replace("\"", "\\\"");
            myFiles = myFiles.Replace("%%USERNAME%%", user.UserFirstname);
            await EmailService.SendEmailAsync("Création d'un nouveau compte - Book Your Car", myFiles, user.UserEmail);



            return Ok();
        }

        [HttpGet, Route("users")]
        public IEnumerable<User> GetUsers()
        {
            var test = System.IO.File.ReadAllLines("./EmailTemplates/Register.html");

            return _context.User.ToList();
        }

        [HttpPost, Route("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(string emailDestinataire)
        {

#if !Debug
            MailjetResponse response =
                await EmailService.SendEmailAsync("Changement de mot de passe", ConstantsEmail.ResetPassword, emailDestinataire);
#endif


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return Ok();
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                return BadRequest();
            }


        }




    }
}