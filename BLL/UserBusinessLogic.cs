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

        async Task<List<ApplicationUser>> GetUsersByRole(string role)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(role);
            return users.ToList();
        }
    }
}
