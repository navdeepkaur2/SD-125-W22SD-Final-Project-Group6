using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly UserBusinessLogic _userBusinessLogic;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _users = userManager;
            _userBusinessLogic = new UserBusinessLogic(context, userManager);
        }
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> projectManager = await _userBusinessLogic.GetUsersByRole("ProjectManager");
            List<ApplicationUser> developers = await _userBusinessLogic.GetUsersByRole("Developer");
            List<ApplicationUser> allUsers = _context.Users.ToList();

            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();
            vm.pms = projectManager;
            vm.devs = developers;
            vm.allUsers = allUsers;
            return View(vm);
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            List<ApplicationUser> allUsers = _context.Users.ToList();

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(u =>
            {
                users.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });
            ViewBag.Users = users;

            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {

            ApplicationUser user = _users.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _users.GetRolesAsync(user);
            if (roleUser.Count == 0)
            {
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            }
            else
            {
                await _users.RemoveFromRoleAsync(user, roleUser.First());
                await _users.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Admin", new { area = "" });
            }
        }
    }
}

