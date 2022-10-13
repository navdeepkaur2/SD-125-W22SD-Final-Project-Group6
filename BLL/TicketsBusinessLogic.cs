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

        public TicketsBusinessLogic(UserManager<ApplicationUser> userManager, ProjectsRepository projectsRepository, TicketsRepository ticketsRepository)
        {
            _userManager = userManager;
            _projectsRepository = projectsRepository;
            _ticketsRepository = ticketsRepository;
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
    }
}
