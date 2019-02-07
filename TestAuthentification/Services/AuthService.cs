
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
        public static A5dContext _context;
        public CustomIdentityErrorDescriber Describer { get; }
               
        //context bdd
        public AuthService(A5dContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
            Describer = errors ?? new CustomIdentityErrorDescriber();
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.User.Where(x => x.UserEmail == email).SingleOrDefaultAsync();
        }

        public async Task Add(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task RemoveAsync(int Id)
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

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var errors = new List<IdentityError>();
            
            if (!CheckEmailUnique(user.UserEmail))
            {
                errors.Add(Describer.DuplicateEmail(user.UserEmail));
            }
            else
            {
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
            }


            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;

        }

        private bool CheckEmailUnique(string userEmail)
        {
            return FindByEmailAsync(userEmail).IsCompletedSuccessfully;
        }
    }
}
