using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Identity.SQLite
{
    /// <summary>
    /// Class that represents the UserLogins table in the Database
    /// </summary>
    public class UserLoginsRepository
    {
        private IdentityEntities _identityEntities;

        /// <summary>
        /// Constructor that takes an IdentityEntities instance
        /// </summary>
        public UserLoginsRepository(IdentityEntities identityEntities)
        {
            _identityEntities = identityEntities;
        }

        /// <summary>
        /// Deletes a login from a user in the UserLogins table
        /// </summary>
        /// <param name="user">User to have login deleted</param>
        /// <param name="login">Login to be deleted from user</param>
        /// <returns></returns>
        public async Task DeleteAsync(IdentityUser user, UserLoginInfo login)
        {
            UserLogins foundLogin = await _identityEntities.UserLogins.SingleOrDefaultAsync(userLogin => userLogin.UserId == user.Id
                && userLogin.LoginProvider == login.LoginProvider
                && userLogin.ProviderKey == login.ProviderKey);

            if (foundLogin != null)
            {
                _identityEntities.UserLogins.Remove(foundLogin);
                await _identityEntities.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all Logins from a user in the UserLogins table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task DeleteAllLoginsAsync(string userId)
        {
            List<UserLogins> foundLogin = await _identityEntities.UserLogins.Where(login => login.UserId == userId).ToListAsync();

            if (foundLogin != null)
            {
                _identityEntities.UserLogins.RemoveRange(foundLogin);
                await _identityEntities.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a new login in the UserLogins table
        /// </summary>
        /// <param name="user">User to have new login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public async Task AddNewLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            if (user != null && login != null)
            {
                _identityEntities.UserLogins.Add(new UserLogins
                {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                });
                await _identityEntities.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Return a userId using a user's login
        /// </summary>
        /// <param name="userLogin">The user's login info</param>
        /// <returns></returns>
        public async Task<string> FindUserIdByLoginAsync(UserLoginInfo userLogin)
        {
            if (userLogin != null)
            {
                UserLogins foundLogin = await _identityEntities.UserLogins.SingleOrDefaultAsync(login => login.LoginProvider == userLogin.LoginProvider
                && login.ProviderKey == userLogin.ProviderKey);

                if (foundLogin != null)
                {
                    return foundLogin.UserId;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns a list of user's logins
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task<List<UserLoginInfo>> FindAllByUserIdAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                List<UserLogins> foundLogins = await _identityEntities.UserLogins.Where(login => login.UserId == userId).ToListAsync();
                return foundLogins
                    .Select(login => new UserLoginInfo(login.LoginProvider, login.ProviderKey))
                    .ToList();
            }
            return new List<UserLoginInfo>();
        }
    }
}