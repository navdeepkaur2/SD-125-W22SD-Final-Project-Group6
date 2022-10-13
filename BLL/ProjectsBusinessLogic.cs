using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectsBusinessLogic
    {
        private readonly ProjectsRepository _projectsRepository;

        public ProjectsBusinessLogic(ProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public Project? FindById(int id)
        {
            return _projectsRepository.FindById(id);
        }

        public List<Project> FindByPage(int page = 1, int count = 10)
        {
            return _projectsRepository.FindList((page - 1) * count, count).ToList();
        }
    }
}
