using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Identity.SQLite
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleRepository _roleRepository;
        private IdentityEntities _identityEntities;

        /// <summary>
        /// Contains all roles in system
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get { return _roleRepository.GetAllRoles<TRole>(); }
        }
        
        /// <summary>
        /// Constructor that takes an IdentityEntities instance
        /// </summary>
        public RoleStore(IdentityEntities identityEntities)
        {
            _identityEntities = identityEntities;
            _roleRepository = new RoleRepository(_identityEntities);
        }

        public async Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await _roleRepository.AddNewRoleAsync(role);
        }

        public async Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await _roleRepository.DeleteRoleAsync(role.Id);
        }

        public async Task<TRole> FindByIdAsync(string roleId)
        {
            return await _roleRepository.GetRoleByIdAsync(roleId) as TRole;
        }

        public async Task<TRole> FindByNameAsync(string roleName)
        {
            return await _roleRepository.GetRoleByNameAsync(roleName) as TRole;
        }

        public async Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            await _roleRepository.UpdateRoleAsync(role);
        }

        public void Dispose()
        {
            if (_identityEntities != null)
            {
                _identityEntities.Dispose();
                _identityEntities = null;
            }
        }
    }
}