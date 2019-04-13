using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            _context.Pole.ToList();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var listUser = await _context.User.ToListAsync();
            if (listUser.Count > 0)
            {
                var model = listUser.Select(x => new UserViewModel()
                {
                    PoleName = x.UserPole != null ? x.UserPole.PoleName : "",
                    UserFirstname = x.UserFirstname,
                    UserEmail = x.UserEmail,
                    UserId = x.UserId,
                    UserRightId = x.UserRightId,
                    UserName = x.UserName,
                    UserPoleId = x.UserPoleId,
                    UserPhone = x.UserPhone,
                    UserNumpermis = x.UserNumpermis
                });
                return Ok(model.ToList());
            }
            var roles = new Dictionary<string, string>();
            roles.Add("message", "Il n'y a pas de Poles.");
            return Ok(roles);


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
                var userInfo = new UserViewModel()
                {
                    UserPoleId = user.UserPoleId,
                    UserFirstname = user.UserFirstname,
                    UserRightId = user.UserRightId,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserNumpermis = user.UserNumpermis,
                    UserPhone = user.UserPhone,
                    PoleName = user.UserPole != null ? user.UserPole.PoleName : "",
                };

                return Ok(userInfo);
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
                    user.UserFirstname = userViewModel.UserFirstname;
                    user.UserName = userViewModel.UserName;
                    user.UserNumpermis = userViewModel.UserNumpermis;
                    user.UserPhone = userViewModel.UserPhone;
                    user.UserPoleId = userViewModel.UserPoleId;
                    user.UserRightId = userViewModel.UserRightId;
                    user.UserPole.PoleName = userViewModel.PoleName;

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok();
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
        //[HttpPost]
        //public async Task<IActionResult> PostUser([FromBody] User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var token = GetToken();
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (TokenService.ValidateToken(token))
        //    {
        //        _context.User.Add(user);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        //    }

        //    return Unauthorized();



        //}

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
                return Ok();
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

        /// <summary>
        /// Retourne le rôle de la personne authentifié
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Retourne la liste des utilisateurs qui n'ont pas encore été validé par l'administrateur
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("userInWaiting")]
        public IActionResult GetUserInWaiting()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateTokenWhereIsAdmin(token))
            {
                List<User> userEnAttente = _context.User.Where(x => !x.UserIsactivated).ToList();

                if (userEnAttente.Count > 0)
                {
                    var model = userEnAttente.Select(x => new UserViewModel()
                    {
                        PoleName = x.UserPole != null ? x.UserPole.PoleName : "",
                        UserFirstname = x.UserFirstname,
                        UserId = x.UserId,
                        UserRightId = x.UserRightId,
                        UserName = x.UserName,
                        UserEmail = x.UserEmail,
                        UserPoleId = x.UserPoleId,
                        UserPhone = x.UserPhone,
                        UserNumpermis = x.UserNumpermis
                    });
                    return Ok(model);
                }
                else
                {
                    //Retourne une liste vide
                    return Ok(new List<User>());
                }

            }

            return Unauthorized();

        }

        /// <summary>
        /// Permet à l'administrateur de valider un utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidateUserInWaiting/{id}")]
        public async Task<IActionResult> ValidateUserInWaiting([FromRoute] int id)
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token) || (!ModelState.IsValid))
            {
                return BadRequest(ModelState);
            }

            if (TokenService.ValidateTokenWhereIsAdmin(token))
            {
                User userValidate = await _context.User.FirstOrDefaultAsync(x => x.UserId == id);

                if (userValidate != null)
                {
                    userValidate.UserIsactivated = true;
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            return Unauthorized();
        }
    }
}

