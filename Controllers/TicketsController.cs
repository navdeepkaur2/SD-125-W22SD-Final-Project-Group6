using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationDbContext _context;
        private readonly TicketsBusinessLogic _ticketsBusinessLogic;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
            _ticketsBusinessLogic = new TicketsBusinessLogic(new TicketsRepository(context));
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
            Project currProject = _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefault(p => p.Id == projId);

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
                ticket.Project = await _context.Projects.FirstAsync(p => p.Id == projId);
                Project currProj = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projId);
                ApplicationUser owner = _context.Users.FirstOrDefault(u => u.Id == userId);
                ticket.Owner = owner;
                _context.Add(ticket);
                currProj.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects", new { area = "" });
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = _context.Users.Where(u => u != ticket.Owner).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket currTicket = await _context.Tickets.Include(t => t.Owner).FirstAsync(t => t.Id == ticketId);
            ApplicationUser currUser = await _context.Users.FirstAsync(u => u.Id == id);
            //To be fixed ASAP
            currTicket.Owner = currUser;
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = ticketId });
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
                    ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == userId);
                    ticket.Owner = currUser;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = ticket.Id });
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == TaskId);

                    newComment.CreatedBy = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    user.Comments.Add(newComment);
                    _context.Comments.Add(newComment);
                    ticket.Comments.Add(newComment);

                    int Id = TaskId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { Id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.RequiredHours = hrs;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
            if (id != null)
            {
                try
                {
                    TicketWatcher newTickWatch = new TicketWatcher();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

                    newTickWatch.Ticket = ticket;
                    newTickWatch.Watcher = user;
                    user.TicketWatching.Add(newTickWatch);
                    ticket.TicketWatchers.Add(newTickWatch);
                    _context.Add(newTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnWatch(int id)
        {
            if (id != null)
            {
                try
                {

                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    TicketWatcher currTickWatch = await _context.TicketWatchers.FirstAsync(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));
                    _context.TicketWatchers.Remove(currTickWatch);
                    ticket.TicketWatchers.Remove(currTickWatch);
                    user.TicketWatching.Remove(currTickWatch);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.Completed = true;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
                    ticket.Completed = false;

                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> DeleteConfirmed(int id, int projId)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.Include(t => t.Project).FirstAsync(p => p.Id == id);
            Project currProj = await _context.Projects.FirstAsync(p => p.Id.Equals(projId));
            if (ticket != null)
            {
                currProj.Tickets.Remove(ticket);
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Projects");
        }

        private bool TicketExists(int id)
        {
            return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

