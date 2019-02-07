
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;

namespace TestAuthentification.Services
{
    public class AuthService 
    {
        private static A5dContext _context;

        //context bdd
        public AuthService(A5dContext context)
        {
            _context = context;
        }

        public async static Task<User> FindByEmailAsync(string email)
        {
            return await _context.User.Where(x => x.UserEmail == email).SingleOrDefaultAsync();
        }

        public async static Task Add(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async static Task<IList<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async static Task RemoveAsync(int Id)
        {
            var itemToRemove = await _context.User.SingleOrDefaultAsync(r => r.UserId == Id);
            if (itemToRemove != null)
            {
                _context.User.Remove(itemToRemove);
                await _context.SaveChangesAsync();
            }
        }

        public static bool CheckPasswordAsync(User user, string password)
        {
            return user.UserPassword.Equals(password);
        }

        public async static Task<IdentityResult> CreateAsync(User user, string password)
        {
            try
            {
                await _context.AddAsync(user);
                return IdentityResult.Success;

            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = e.Source,
                    Description = e.Message
                });
            }
            

        }




    }
}
