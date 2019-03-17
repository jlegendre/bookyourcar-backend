using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.Services;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly A5dContext _context;

        public UserController(A5dContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUser()
        {
            return _context.User;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = GetToken();
            if (TokenService.ValidateToken(token))
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }

            return Unauthorized();
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != userViewModel.UserId)
            {
                return BadRequest();
            }
            var user = _context.User.SingleOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateToken(token))
            {

                try
                {
                    user.UserEmail = userViewModel.UserEmail;
                    user.UserPassword = userViewModel.UserPassword;
                    user.UserFirstname = userViewModel.UserFirstname;
                    user.UserName = userViewModel.UserName;
                    user.UserNumpermis = userViewModel.UserNumpermis;
                    user.UserPhone = userViewModel.UserPhone;
                    user.UserPoleId = userViewModel.UserPoleId;
                    user.UserRightId = userViewModel.UserRightId;

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Unauthorized();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateToken(token))
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }

            return Unauthorized();



        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateToken(token))
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }

            return Unauthorized();

        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        /// <summary>
        /// permet de récuperer le token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            var token = Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer"))
            {
                var tab = token.Split(" ");
                token = tab[1];
            }

            return token;
        }
        [HttpGet, Route("userRole")]
        public IActionResult GetUserRole()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token) || (!ModelState.IsValid))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateToken(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var simplePrinciple = handler.ReadJwtToken(token);
                var role = simplePrinciple.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                var roles = new Dictionary<string, string>();
                roles.Add("role", role);

                return Ok(roles);
            }

            return Unauthorized();

        }
    }


}