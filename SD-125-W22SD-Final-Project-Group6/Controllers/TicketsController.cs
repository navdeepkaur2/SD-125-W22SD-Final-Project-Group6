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
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserBusinessLogic _userBusinessLogic;
        private readonly ProjectsBusinessLogic _projectsBusinessLogic;
        private readonly TicketsBusinessLogic _ticketsBusinessLogic;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userBusinessLogic = new UserBusinessLogic(userManager);
            _projectsBusinessLogic = new ProjectsBusinessLogic(userManager, new ProjectsRepository(context), new TicketsRepository(context));
            _ticketsBusinessLogic = new TicketsBusinessLogic(userManager, new ProjectsRepository(context), new TicketsRepository(context), new CommentsRepository(context));
        }

        // GET: Tickets
        public IActionResult Index()
        {
            List<Ticket> tickets = _ticketsBusinessLogic.GetAll();
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<SelectListItem> currUsers = new List<SelectListItem>();
            ticket.Project!.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projId)
        {
            Project? currProject = _projectsBusinessLogic.FindById(projId);

            if (currProject == null)
            {
                return NotFound();
            }

            List<SelectListItem> currUsers = new List<SelectListItem>();
            currProject.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            ViewBag.Projects = currProject;
            ViewBag.Users = currUsers;

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            {
                await _ticketsBusinessLogic.Create(ticket, projId, userId);

                return RedirectToAction("Index", "Projects", new { area = "" });
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<ApplicationUser> developers = await _userBusinessLogic.GetUsersByRole("Developer");

            List<ApplicationUser> results = developers.Where(u => u != ticket.Owner).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = await _userBusinessLogic.FindById(userId);
                    ticket.Owner = user;
                    _ticketsBusinessLogic.Update(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_ticketsBusinessLogic.Exists(ticket.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Edit), new { id = ticket.Id });
            }

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int? TaskId, string? TaskText)
        {
            if (TaskId != null && TaskText != null)
            {
                try
                {
                    ApplicationUser user = await _userBusinessLogic.FindByName(User.Identity.Name);

                    await _ticketsBusinessLogic.AddComment(user.Id, (int)TaskId, TaskText);

                    int Id = (int)TaskId;
                    return RedirectToAction("Details", new { Id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateHrs(int? id, int? hrs)
        {
            if (id != null && hrs != null)
            {
                try
                {
                    Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

                    if (ticket == null)
                    {
                        throw new ArgumentException("ticket is not found");
                    }

                    ticket.RequiredHours = (int)hrs;

                    _ticketsBusinessLogic.Update(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToWatchers(int? id)
        {
            if (id != null)
            {
                try
                {
                    ApplicationUser user = await _userBusinessLogic.FindByName(User.Identity.Name);

                    await _ticketsBusinessLogic.AddToWatchers(user.Id, (int)id);

                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnWatch(int? id)
        {
            if (id != null)
            {
                try
                {
                    ApplicationUser user = await _userBusinessLogic.FindByName(User.Identity.Name);
                    await _ticketsBusinessLogic.RemoveFromWatchers(user.Id, (int)id);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsCompleted(int? id)
        {
            if (id != null)
            {
                try
                {
                    Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

                    if (ticket == null)
                    {
                        throw new ArgumentException("ticket not found");
                    }

                    ticket.Completed = true;
                    _ticketsBusinessLogic.Update(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int? id)
        {
            if (id != null)
            {
                try
                {
                    Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

                    if (ticket == null)
                    {
                        throw new ArgumentException("ticket not found");
                    }

                    ticket.Completed = false;
                    _ticketsBusinessLogic.Update(ticket);

                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction("Index");
        }


        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket? ticket = _ticketsBusinessLogic.FindById((int)id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public IActionResult DeleteConfirmed(int id, int projId)
        {
            _ticketsBusinessLogic.Delete(id);

            return RedirectToAction("Index", "Projects");
        }
    }
}

