using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestAuthentification.Models;

namespace TestAuthentification.Services
{
    public class UserService
    {
        private readonly A5dContext _context;

        public UserService(A5dContext context)
        {
            _context = context;
        }

        




    }
}