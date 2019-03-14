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

        [HttpGet]
        public async Task<IActionResult> GetUser(string token)
        {
            if (token == null)
            {
                return Unauthorized();
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var simplePrinciple = handler.ReadJwtToken(token);
                var email = simplePrinciple.Claims.First(x => x.Type == ClaimTypes.Email).Value;

                if (TokenService.ValidateToken(token))
                {
                    var user = await _context.User.Where(x => x.UserEmail == email).SingleOrDefaultAsync();

                    return Ok(user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Unauthorized();

        }

        [HttpPut]
        public IActionResult UpdateUser(string token, UserViewModel userViewModel)
        {
            var user = _context.User.SingleOrDefault(x => x.UserEmail == userViewModel.UserEmail);

            if (user == null)
            {
                return NotFound();
            }
            user.UserEmail = userViewModel.UserEmail;
            user.UserPassword = userViewModel.UserPassword;
            user.UserFirstname = userViewModel.UserFirstname;
            user.UserName = userViewModel.UserName;
            user.UserNumpermis = userViewModel.UserNumpermis;
            user.UserPhone = userViewModel.UserPhone;
            user.UserPoleId = userViewModel.UserPoleId;
            user.UserRightId = userViewModel.UserRightId;
            try
            {
                if (TokenService.ValidateToken(token))
                {
                    _context.User.Update(user);
                    _context.SaveChanges();
                    return Ok(user);
                }

                return Unauthorized();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string token, int id)
        {
            if (TokenService.ValidateToken(token))
            {
                var user = _context.User.Find(id);
                _context.User.Remove(user);
                _context.SaveChanges();
                return Ok();
            }

            return Unauthorized();

        }


    }
}