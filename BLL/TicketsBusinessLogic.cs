using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketsBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectsRepository _projectsRepository;
        private readonly TicketsRepository _ticketsRepository;
        private readonly CommentsRepository _commentsRepository;

        public TicketsBusinessLogic(UserManager<ApplicationUser> userManager, ProjectsRepository projectsRepository, TicketsRepository ticketsRepository, CommentsRepository commentsRepository)
        {
            _userManager = userManager;
            _projectsRepository = projectsRepository;
            _ticketsRepository = ticketsRepository;
            _commentsRepository = commentsRepository;
        }

        public async Task Create(Ticket ticket, int projectId, string userId)
        {
            Project? project = _projectsRepository.FindById(projectId);
            ApplicationUser owner = await _userManager.FindByIdAsync(userId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            ticket.Project = project;
            ticket.Owner = owner;

            _ticketsRepository.Create(ticket);
            _ticketsRepository.Save();
        }

        public Ticket? FindById(int id)
        {
            return _ticketsRepository.FindById(id);
        }

        public List<Ticket> GetAll()
        {
            return _ticketsRepository.GetAll().ToList();
        }

        public async Task Update(Ticket ticket, string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            ticket.Owner = user;

            _ticketsRepository.Update(ticket);
            _ticketsRepository.Save();
        }

        public bool Exists(int id)
        {
            Ticket? ticket = _ticketsRepository.FindById(id);
            return ticket != null;
        }

        public async Task AddComment(string userId, int ticketId, string description)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            Ticket? ticket = _ticketsRepository.FindById(ticketId);

            if (ticket == null)
            {
                throw new ArgumentException("ticketId does not exist");
            }

            Comment newComment = new Comment();
            newComment.CreatedBy = user;
            newComment.Description = description;
            newComment.Ticket = ticket;

            _commentsRepository.Create(newComment);
            _commentsRepository.Save();
        }
    }
}
