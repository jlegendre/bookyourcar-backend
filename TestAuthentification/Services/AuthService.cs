﻿
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

        /// <summary>
        /// Recupère l'utilisateur en fonction de son email 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.User.Where(x => x.UserEmail == email).SingleOrDefaultAsync();
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
        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var errors = new List<IdentityError>();
            
            if (!CheckEmailUnique(user.UserEmail))
            {
                errors.Add(Describer.DuplicateEmail(user.UserEmail));
            }
            if (string.IsNullOrEmpty(user.UserEmail))
            {
                errors.Add(Describer.InvalidEmail(user.UserEmail));
            }
            else
            {
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
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
            return FindByEmailAsync(userEmail).IsCompletedSuccessfully;
        }
    }
}
