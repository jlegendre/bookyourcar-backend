
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
        private readonly A5dContext _context;
        public readonly CustomIdentityErrorDescriber Describer;

        //context bdd
        public AuthService(A5dContext context, CustomIdentityErrorDescriber errors = null)
        {

            _context = context;

            Describer = errors ?? new CustomIdentityErrorDescriber();
        }

        /// <summary>
        /// Recupère l'utilisateur en fonction de son email 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User FindByEmail(string email)
        {
            // TODO A REVOIR  car lorsqu'on appel la meethode et que UserRight est null --> ça plante donc a revoir
            var user = _context.User
                .Include(i => i.UserRight.UserUserRight).SingleOrDefault(x => x.UserEmail == email);
            return user;

        }

        /// <summary>
        /// Recupère la liste des utilisateurs
        /// </summary>
        /// <returns></returns>
        public async Task<IList<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync();
        }

        /// <summary>
        /// Supprime un utilisateur en fonction de son Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(int Id)
        {
            var itemToRemove = await _context.User.SingleOrDefaultAsync(r => r.UserId == Id);
            if (itemToRemove != null)
            {
                _context.User.Remove(itemToRemove);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Regarde si le password correspond à celui à l'utilisateur
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(User user, string password)
        {
            return user.UserPassword.Equals(password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public IdentityResult VerifUser(User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (CheckEmailUnique(user.UserEmail))
            {
                errors.Add(Describer.DuplicateEmail(user.UserEmail));
            }
            else if (string.IsNullOrEmpty(user.UserEmail))
            {
                errors.Add(Describer.InvalidEmail(user.UserEmail));
            }

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;

        }

        /// <summary>
        /// Regarde si l'email est bien unique
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private bool CheckEmailUnique(string userEmail)
        {
            var user = FindByEmail(userEmail);

            if (user != null)
            {
                return true;
            }

            return false;
        }

        public IdentityResult AddToRoleAdminAsync(User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (user.UserRightId != 0 || user.UserRightId != null)
            {
                errors.Add(Describer.UserAlreadyInRole(user.UserRight.RightLabel));
            }
            else
            {
                user.UserRightId = 1;
            }

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;

        }

        public IdentityResult AddToRoleUserAsync(User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (user.UserRightId != null)
            {
                errors.Add(Describer.UserAlreadyInRole(user.UserRight.RightLabel));
            }
            else
            {
                user.UserRightId = 2;
            }

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;

        }

    }
}
