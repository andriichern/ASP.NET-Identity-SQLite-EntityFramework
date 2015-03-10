using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Identity.SQLite
{
    /// <summary>
    /// Class that is responsible for user roles
    /// </summary>
    public class UserRolesRepository
    {
        private IdentityEntities _identityEntities;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        public UserRolesRepository(IdentityEntities identityEntities)
        {
            _identityEntities = identityEntities;
        }

        /// <summary>
        /// Returns user's role name
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task<string> FindUserRoleByUserIdAsync(string userId)
        {
            Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(user => user.Id == userId);

            if (currentUser != null && currentUser.Roles != null)
            {
                if (!String.IsNullOrEmpty(currentUser.Roles.Name))
                {
                    return currentUser.Roles.Name;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Sets to user given role ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task SetUserRoleAsync(string userId, string roleId)
        {
            if (!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(roleId))
            {
                Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(user => user.Id == userId);
                if (currentUser != null)
                {
                    currentUser.RoleId = roleId;
                    await _identityEntities.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Sets roleId of a given user to null
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task RemoveUserRoleAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(user => user.Id == userId);
                if (currentUser != null)
                {
                    currentUser.RoleId = null;
                    await _identityEntities.SaveChangesAsync();
                }
            }
        }
    }
}