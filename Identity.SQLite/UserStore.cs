using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Identity.SQLite
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store iterfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        private IdentityEntities _identityEntities;
        private UserRepository<TUser> _userRepository;
        private RoleRepository _roleRepository;
        private UserRolesRepository _userRolesRepository;
        private UserClaimsRepository _userClaimsRepository;
        private UserLoginsRepository _userLoginsRepository;

        /// <summary>
        /// Contains all users registered in system
        /// </summary>
        public IQueryable<TUser> Users
        {
            get { return _userRepository.GetUsers(); }
        }

        /// <summary>
        /// Constructor that takes an IdentityEntities instance
        /// </summary>
        public UserStore(IdentityEntities identityEntities)
        {
            _identityEntities = identityEntities;
            _userRepository = new UserRepository<TUser>(_identityEntities);
            _roleRepository = new RoleRepository(_identityEntities);
            _userRolesRepository = new UserRolesRepository(_identityEntities);
            _userClaimsRepository = new UserClaimsRepository(_identityEntities);
            _userLoginsRepository = new UserLoginsRepository(_identityEntities);
        }

        /// <summary>
        /// Creates a new TUser instance
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _userRepository.AddNewUserAsync(user);
        }

        /// <summary>
        /// Returns a TUser instance based on a userId
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <returns></returns>
        public async Task<TUser> FindByIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return await _userRepository.GetUserByIdAsync(userId);
        }

        /// <summary>
        /// Returns a TUser instance based on a userName
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public async Task<TUser> FindByNameAsync(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            return await _userRepository.GetUserByNameAsync(userName);
        }

        /// <summary>
        /// Updates current user info
        /// </summary>
        /// <param name="user">TUser to be updated</param>
        /// <returns></returns>
        public async Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            await _userRepository.UpdateUserAsync(user);
        }

        public void Dispose()
        {
            if (_identityEntities != null)
            {
                _identityEntities.Dispose();
                _identityEntities = null;
            }
        }

        /// <summary>
        /// Adds a claim for the given user
        /// </summary>
        /// <param name="user">User to have claim added</param>
        /// <param name="claim">Claim to be added</param>
        /// <returns></returns>
        public async Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            await _userClaimsRepository.AddNewClaimsAsync(claim, user.Id);
        }

        /// <summary>
        /// Returns all claims for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user != null)
            {
                ClaimsIdentity identity = await _userClaimsRepository.FindByUserIdAsync(user.Id);

                return await Task.FromResult(identity.Claims.ToList());
            }
            return await Task.FromResult<List<Claim>>(null);
        }

        /// <summary>
        /// Removes a claim from a given user
        /// </summary>
        /// <param name="user">User to have claim removed</param>
        /// <param name="claim">Claim to be removed</param>
        /// <returns></returns>
        public async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            await _userClaimsRepository.DeleteClaimAsync(user, claim);
        }

        /// <summary>
        /// Adds a Login for a given User
        /// </summary>
        /// <param name="user">User to have login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            await _userLoginsRepository.AddNewLoginAsync(user, login);
        }

        /// <summary>
        /// Returns a TUser based on the Login info
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = await _userLoginsRepository.FindUserIdByLoginAsync(login);
            
            if (userId != null)
            {
                return await _userRepository.GetUserByIdAsync(userId);
            }

            return await Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return await _userLoginsRepository.FindAllByUserIdAsync(user.Id);
        }

        /// <summary>
        /// Deletes given login for a given TUser
        /// </summary>
        /// <param name="user">User to have login removed</param>
        /// <param name="login">Login to be removed</param>
        /// <returns></returns>
        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            await _userLoginsRepository.DeleteAsync(user, login);
        }

        /// <summary>
        /// Sets to given TUser given role
        /// </summary>
        /// <param name="user">User to have role added</param>
        /// <param name="roleName">Name of the role to be added to user</param>
        /// <returns></returns>
        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            string roleId = await _roleRepository.GetRoleIdAsync(roleName);
            if (!String.IsNullOrEmpty(roleId))
            {
                await _userRolesRepository.SetUserRoleAsync(user.Id, roleId);
            }
        }

        /// <summary>
        /// Returns the roles for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            string currentUserRole = await _userRolesRepository.FindUserRoleByUserIdAsync(user.Id);
            
            if (!String.IsNullOrEmpty(currentUserRole))
            {
                return new List<string> { currentUserRole };
            }
            return new List<string>();
        }

        /// <summary>
        /// Verifies if a user is in a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            string currentUserRole = await _userRolesRepository.FindUserRoleByUserIdAsync(user.Id);
            if (StringComparer.Ordinal.Compare(currentUserRole, role) == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes user's role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task RemoveFromRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }
            await _userRolesRepository.RemoveUserRoleAsync(user.Id);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                await _userRepository.DeleteUserAsync(user);
            }
        }

        /// <summary>
        /// Returns the PasswordHash of a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return await _userRepository.GetPasswordHashAsync(user.Id);
        }

        /// <summary>
        /// Verifies if user has password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return !String.IsNullOrEmpty(await _userRepository.GetPasswordHashAsync(user.Id));
        }

        /// <summary>
        /// Sets the password hash for a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException("passwordHash");
            }
            await Task.Run(() => user.PasswordHash = passwordHash);
        }

        /// <summary>
        ///  Sets the security stamp to a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public async Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrEmpty(stamp))
            {
                throw new ArgumentNullException("stamp");
            }
            await Task.Run(() => user.SecurityStamp = stamp);
        }

        /// <summary>
        /// Gets security stampof a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Sets email to a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SetEmailAsync(TUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            user.Email = email;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets email of a given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Gets if user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Sets whether user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            return await _userRepository.GetUserByEmailAsync(email);
        }

        /// <summary>
        /// Sets user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException("phoneNumber");
            }
            user.PhoneNumber = phoneNumber;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets user phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Gets if user phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets if phone number is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Sets two factor authentication is enabled for given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets if two factor authentication is enabled on the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Gets user lock out end date
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEndDateUtc.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                : new DateTimeOffset());
        }


        /// <summary>
        /// Sets user lockout end date
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Increments failed access count of given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            await  _userRepository.UpdateUserAsync(user);
            return user.AccessFailedCount;
        }

        /// <summary>
        /// Resets failed access count of given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets failed access count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Gets if lockout is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Sets lockout enabled value for given TUser
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            await _userRepository.UpdateUserAsync(user);
        }
    }
}