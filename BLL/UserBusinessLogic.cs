using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserBusinessLogic
    {
        public ApplicationDbContext _context;
        public UserManager<ApplicationUser> _userManager;

        public UserBusinessLogic(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            ICollection<string> userRoles = await _userManager.GetRolesAsync(user);

            foreach (string userRole in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, userRole);
            }

            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
