using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Security.Claims;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectsBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectsRepository _projectsRepository;
        private readonly TicketsRepository _ticketsRepository;

        public ProjectsBusinessLogic(UserManager<ApplicationUser> userManager, ProjectsRepository projectsRepository, TicketsRepository ticketsRepository)
        {
            _userManager = userManager;
            _projectsRepository = projectsRepository;
            _ticketsRepository = ticketsRepository;
        }

        public async Task Create(string userId, Project project, List<string> assignedUserIds)
        {
            project.CreatedBy = await _userManager.FindByIdAsync(userId);

            foreach (string assignedUserId in assignedUserIds)
            {
                ApplicationUser assignedUser = await _userManager.FindByIdAsync(assignedUserId);

                UserProject userProject = new UserProject();
                userProject.ApplicationUser = assignedUser;
                userProject.UserId = assignedUser.Id;
                userProject.Project = project;

                project.AssignedTo.Add(userProject);
                _projectsRepository.Create(project);
            }

            _projectsRepository.Save();
        }

        public Project? FindById(int id)
        {
            return _projectsRepository.FindById(id);
        }

        public List<Project> FindByPage(int page = 1, int count = 10)
        {
            return _projectsRepository.FindList((page - 1) * count, count).ToList();
        }

        public void Delete(int projectId)
        {
            Project? project = _projectsRepository.FindById(projectId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            foreach (Ticket ticket in project.Tickets)
            {
                _ticketsRepository.Delete(ticket);
            }
            _ticketsRepository.Save();

            foreach (UserProject userProject in project.AssignedTo)
            {
                _projectsRepository.RemoveAssignedUser((int)userProject.ProjectId, (string)userProject.UserId);
            }
            _projectsRepository.Save();

            _projectsRepository.Delete(project);
            _projectsRepository.Save();
        }

        public int Count()
        {
            return _projectsRepository.Count();
        }

        public bool Exists(int id)
        {
            Project? project = _projectsRepository.FindById(id);

            return project != null;
        }

        public void RenameProject(int projectId, string name)
        {
            Project? project = _projectsRepository.FindById(projectId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            project.ProjectName = name;

            _projectsRepository.Update(project);
            _projectsRepository.Save();
        }

        public async Task AddAssignedUsers(int projectId, List<string> assignedUserIds)
        {
            Project? project = _projectsRepository.FindById(projectId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            foreach (string userId in assignedUserIds)
            {
                ApplicationUser assignedUser = await _userManager.FindByIdAsync(userId);

                UserProject userProject = new UserProject();
                userProject.ApplicationUser = assignedUser;
                userProject.UserId = assignedUser.Id;
                userProject.Project = project;

                if (project.AssignedTo.FirstOrDefault(up => up.UserId == assignedUser.Id) == null)
                {
                    project.AssignedTo.Add(userProject);
                }
            }

            _projectsRepository.Update(project);
            _projectsRepository.Save();
        }

        public void RemoveAssignedUser(int projectId, string userId)
        {
            _projectsRepository.RemoveAssignedUser(projectId, userId);
            _projectsRepository.Save();
        }
    }
}
