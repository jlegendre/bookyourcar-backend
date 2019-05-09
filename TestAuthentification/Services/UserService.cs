using System;
using System.Collections.Generic;
using System.Linq;
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

        public User GetUserConnected(string authToken)
        {
            if (TokenService.ValidateToken(authToken))
            {
                User user = 

                return user;
            }

        }




    }
}