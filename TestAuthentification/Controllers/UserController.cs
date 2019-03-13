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

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        public readonly A5dContext _context;

        public UserController(A5dContext context)
        {
            _context = context;
        }

        [HttpGet, Route("user")]
        public async Task<IActionResult> GetUserInfo(string token)
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

    }
}