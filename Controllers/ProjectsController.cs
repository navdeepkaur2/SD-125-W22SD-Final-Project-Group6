using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using X.PagedList;
using X.PagedList.Mvc;


namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly UserBusinessLogic _userBusinessLogic;
        private readonly ProjectsBusinessLogic _projectsBusinessLogic;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _users = userManager;
            _userBusinessLogic = new UserBusinessLogic(userManager);
            _projectsBusinessLogic = new ProjectsBusinessLogic(userManager, new ProjectsRepository(context));
        }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder = null, int page = 1, bool sort = false, string? userId = null)
        {
            List<ApplicationUser> developers = await _userBusinessLogic.GetUsersByRole("Developer");
            List<SelectListItem> users = new List<SelectListItem>();
            developers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;

            List<Project> projects = _projectsBusinessLogic.FindByPage(page);
            switch (sortOrder)
            {
                case "Priority":
                    projects.ForEach(p =>
                    {
                        if (sort)
                        {
                            p.Tickets = p.Tickets.OrderByDescending(t => t.TicketPriority).ToList();

                        }
                        else
                        {
                            p.Tickets = p.Tickets.OrderBy(t => t.TicketPriority).ToList();
                        }
                    });
                    break;
                case "RequiredHrs":
                    projects.ForEach(p =>
                    {
                        if (sort)
                        {
                            p.Tickets = p.Tickets.OrderByDescending(t => t.RequiredHours).ToList();

                        }
                        else
                        {
                            p.Tickets = p.Tickets.OrderBy(t => t.RequiredHours).ToList();

                        }
                    });
                    break;
                case "Completed":
                    projects.ForEach(p =>
                    {
                        p.Tickets = p.Tickets.Where(t => t.Completed == true).ToList();
                    });
                    break;
                default:
                    projects = projects.OrderBy(p => p.ProjectName).ToList();
                    if (userId != null)
                    {
                        projects.ForEach(p =>
                        {
                            p.Tickets = p.Tickets.Where(t => t.Owner?.Id == userId).ToList();
                        });
                    }
                    break;
            }

            ApplicationUser user = await _userBusinessLogic.FindByName(User.Identity.Name);
            List<string> roles = await _userBusinessLogic.GetRoles(user.Id);

            if (roles.Contains("Developer"))
            {
                projects = projects.Where(p => p.AssignedTo.Select(projectUser => projectUser.UserId).Contains(user.Id)).ToList();
            }

            return View(projects.ToPagedList());
        }

        // GET: Projects/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = _projectsBusinessLogic.FindById((int)id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        {
            if (id == null)
            {
                return NotFound();
            }
            UserProject currUserProj = await _context.UserProjects.FirstAsync(up => up.ProjectId == projId && up.UserId == id);
            _context.UserProjects.Remove(currUserProj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = projId });
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            List<ApplicationUser> developers = await _userBusinessLogic.GetUsersByRole("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            developers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] Project project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                await _projectsBusinessLogic.Create(User, project, userIds);

                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project? project = _projectsBusinessLogic.FindById((int)id);

            if (project == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = await _userBusinessLogic.GetUsersByRole("Developer");

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userIds, [Bind("Id,ProjectName")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userIds.ForEach((user) =>
                    {
                        ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == user);
                        UserProject newUserProj = new UserProject();
                        newUserProj.ApplicationUser = currUser;
                        newUserProj.UserId = currUser.Id;
                        newUserProj.Project = project;
                        project.AssignedTo.Add(newUserProj);
                    });
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.Include(p => p.Tickets).FirstAsync(p => p.Id == id);
            if (project != null)
            {
                List<Ticket> tickets = project.Tickets.ToList();
                tickets.ForEach(ticket =>
                {
                    _context.Tickets.Remove(ticket);
                });
                await _context.SaveChangesAsync();
                List<UserProject> userProjects = _context.UserProjects.Where(up => up.ProjectId == project.Id).ToList();
                userProjects.ForEach(userProj =>
                {
                    _context.UserProjects.Remove(userProj);
                });

                _context.Projects.Remove(project);


            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
