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

        public ProjectsBusinessLogic(UserManager<ApplicationUser> userManager, ProjectsRepository projectsRepository)
        {
            _userManager = userManager;
            _projectsRepository = projectsRepository;
        }

        public async Task Create(ClaimsPrincipal user, Project project, List<string> assignedUserIds)
        {
            project.CreatedBy = await _userManager.GetUserAsync(user);

            foreach (string userId in assignedUserIds)
            {
                ApplicationUser assignedUser = await _userManager.FindByIdAsync(userId);

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

        public void RemoveAssignedUser(string userId, int projectId)
        {
            Project? project = _projectsRepository.FindById(projectId);

            if (project == null)
            {
                throw new ArgumentException("projectId does not exist");
            }

            project.AssignedTo = project.AssignedTo.Where(up => !(up.UserId == userId && up.ProjectId == projectId)).ToList();

            _projectsRepository.Update(project);
            _projectsRepository.Save();
        }
    }
}
