using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.SQLite
{
    /// <summary>
    /// Class that represents the Users table in the Database
    /// </summary>
    internal class UserRepository<TUser>
        where TUser :IdentityUser
    {
        private readonly IdentityEntities _identityEntities;

        /// <summary>
        /// Constructor that takes a IdentityEntities instance 
        /// </summary>
        public UserRepository(IdentityEntities identityEntities)
        {
            _identityEntities = identityEntities;
        }

        /// <summary>
        /// Returns all users in Users table
        /// </summary>
        /// <returns></returns>
        public IQueryable<TUser> GetUsers()
        {
            return _identityEntities.Users as IQueryable<TUser>;
        }

        /// <summary>
        /// Returns user's Name using user's ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetUserNameByUserIdAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (currentUser != null)
                {
                    return currentUser.UserName;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns a User ID using a user name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public async Task<string> GetUserIdByUserNameAsync(string userName)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.UserName == userName);
                if (currentUser != null)
                {
                    return currentUser.Id;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns an TUser instance using a user's ID
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task<TUser> GetUserByIdAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    return await CreateUserAsync(foundUser);
                }
            }
            return await Task.FromResult<TUser>(null);
            //return null;
        }

        /// <summary>
        /// Returns an TUser instance using a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public async Task<TUser> GetUserByNameAsync(string userName)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.UserName == userName);
                if (foundUser != null)
                {
                    return await CreateUserAsync(foundUser);
                }
            }
            return await Task.FromResult<TUser>(null);
            //return null;
        }

        /// <summary>
        /// Returns TUser instance using an email of user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<TUser> GetUserByEmailAsync(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (foundUser != null)
                {
                    return await CreateUserAsync(foundUser);
                }
            }
            //return new IdentityUser();
            return await Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Return user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task<string> GetPasswordHashAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    return foundUser.PasswordHash;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public async Task SetPasswordHashAsync(string userId, string passwordHash)
        {
            if (!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(passwordHash))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    foundUser.PasswordHash = passwordHash;
                    await _identityEntities.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Returns the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetSecurityStampAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    return foundUser.SecurityStamp;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Sets the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public async Task SetSecurityStampAsync(string userId, string stamp)
        {
            if (!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(stamp))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    foundUser.SecurityStamp = stamp;
                    await _identityEntities.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Adds a new user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task AddNewUserAsync(IdentityUser user)
        {
            if (user != null)
            {
                _identityEntities.Users.Add(new Users
                {
                    Id = user.Id,
                    RoleId = user.RoleId,
                    UserName = user.UserName,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp,
                    Email = user.Email,
                    EmailConfirmed = Convert.ToInt32(user.EmailConfirmed),
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = Convert.ToInt32(user.PhoneNumberConfirmed),
                    AccessFailedCount = user.AccessFailedCount,
                    LockoutEnabled = Convert.ToInt32(user.LockoutEnabled),
                    LockoutEndDateUtc = user.LockoutEndDateUtc.HasValue ? user.LockoutEndDateUtc.ToString() : null,
                    TwoFactorEnabled = Convert.ToInt32(user.TwoFactorEnabled)
                });
                await _identityEntities.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(TUser user)
        {
            if (user != null)
            {
                await DeleteUserAsync(user.Id);
            }
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(TUser user)
        {
            if (user != null)
            {
                Users currentUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == user.Id);

                if (currentUser != null)
                {
                    currentUser.UserName = user.UserName;
                    currentUser.PasswordHash = user.PasswordHash;
                    currentUser.SecurityStamp = user.SecurityStamp;
                    currentUser.Email = user.Email;
                    currentUser.EmailConfirmed = Convert.ToInt32(user.EmailConfirmed);
                    currentUser.PhoneNumber = user.PhoneNumber;
                    currentUser.PhoneNumberConfirmed = Convert.ToInt32(user.PhoneNumberConfirmed);
                    currentUser.LockoutEnabled = Convert.ToInt32(user.LockoutEnabled);
                    currentUser.LockoutEndDateUtc = user.LockoutEndDateUtc.ToString();
                    currentUser.AccessFailedCount = user.AccessFailedCount;
                    currentUser.TwoFactorEnabled = Convert.ToInt32(user.TwoFactorEnabled);

                    await _identityEntities.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        private async Task DeleteUserAsync(string userId)
        {
            if (!String.IsNullOrEmpty(userId))
            {
                Users foundUser = await _identityEntities.Users.SingleOrDefaultAsync(u => u.Id == userId);
                if (foundUser != null)
                {
                    _identityEntities.Users.Remove(foundUser);
                    await _identityEntities.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Creates new instance of TUser from found entry in database
        /// </summary>
        /// <param name="foundUser"></param>
        /// <returns></returns>
        private async Task<TUser> CreateUserAsync(Users foundUser)
        {
            TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
            user.Id = foundUser.Id;
            user.RoleId = foundUser.RoleId;
            user.UserName = foundUser.UserName;
            user.Email = String.IsNullOrEmpty(foundUser.Email) ? null : foundUser.Email;
            user.EmailConfirmed = foundUser.EmailConfirmed == 1;
            user.PasswordHash = String.IsNullOrEmpty(foundUser.PasswordHash) ? null : foundUser.PasswordHash;
            user.SecurityStamp = String.IsNullOrEmpty(foundUser.SecurityStamp) ? null : foundUser.SecurityStamp;
            user.PhoneNumber = String.IsNullOrEmpty(foundUser.PhoneNumber) ? null : foundUser.PhoneNumber;
            user.PhoneNumberConfirmed = foundUser.PhoneNumberConfirmed == 1;
            user.TwoFactorEnabled = foundUser.TwoFactorEnabled == 1;
            user.LockoutEnabled = foundUser.LockoutEnabled == 1;
            user.LockoutEndDateUtc = String.IsNullOrEmpty(foundUser.LockoutEndDateUtc)
                ? DateTime.Now
                : DateTime.Parse(foundUser.LockoutEndDateUtc);
            user.AccessFailedCount = foundUser.AccessFailedCount;
            return await Task.FromResult(user);
        }
    }
}