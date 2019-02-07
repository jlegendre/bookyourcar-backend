using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TestAuthentification.Models;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly A5dContext _context;
        private AuthService _authService;

        public AuthController(A5dContext context)
        {
            _context = context;
            _authService = new AuthService(_context);
        }

        public AuthController()
        {

        }

        // GET api/values
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel loginViewModel)
        {
            if (loginViewModel == null && !ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            User myUser = await _authService.FindByEmailAsync(loginViewModel.Email);
                       
            if (myUser != null && _authService.CheckPasswordAsync(myUser, loginViewModel.Password))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User() { UserPassword = registerViewModel.Password, UserEmail = registerViewModel.Email };

            var result = await _authService.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }


        [Authorize]
        [HttpGet, Route("users")]
        public IEnumerable<User> getUsers()
        {
            //var test = _context.User.ToList();


            return null;

        }
    }
}