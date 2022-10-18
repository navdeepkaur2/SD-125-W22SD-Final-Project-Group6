using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserBusinessLogic
    {
        public UserManager<ApplicationUser> _userManager;

        public UserBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> FindByName(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<List<ApplicationUser>> GetUsersByRole(string role)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(role);
            return users.ToList();
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public async Task AssignRole(string userId, string role)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User with specified userId not found.");
            }

            ICollection<string> userRoles = await _userManager.GetRolesAsync(user);

            foreach (string userRole in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, userRole);
            }

            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<List<string>> GetRoles(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User with specified userId not found.");
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
