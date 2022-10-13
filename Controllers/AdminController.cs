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
        private readonly UserBusinessLogic _userBusinessLogic;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userBusinessLogic = new UserBusinessLogic(userManager);
        }

        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> projectManager = await _userBusinessLogic.GetUsersByRole("ProjectManager");
            List<ApplicationUser> developers = await _userBusinessLogic.GetUsersByRole("Developer");
            List<ApplicationUser> allUsers = _userBusinessLogic.GetAllUsers();

            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();
            vm.pms = projectManager;
            vm.devs = developers;
            vm.allUsers = allUsers;

            return View(vm);
        }

        public IActionResult ReassignRole()
        {
            List<ApplicationUser> allUsers = _userBusinessLogic.GetAllUsers();

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
            await _userBusinessLogic.AssignRole(userId, role);

            return RedirectToAction("Index", "Admin", new { area = "" });
        }
    }
}

