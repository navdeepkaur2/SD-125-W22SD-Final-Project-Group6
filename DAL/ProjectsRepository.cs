using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class ProjectsRepository : IRepository<Project>
    {
        private readonly ApplicationDbContext _context;

        public ProjectsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Project entity)
        {
            _context.Projects.Add(entity);
        }

        public Project? FindById(int id)
        {
            return _context.Projects.Find(id);
        }

        public Project? Find(Func<Project, bool> predicate)
        {
            return _context.Projects.Find(predicate);
        }

        public ICollection<Project> FindList(Func<Project, bool> predicate)
        {
            return _context.Projects.Where(predicate).ToList();
        }

        public ICollection<Project> GetAll()
        {
            return _context.Projects.ToList();
        }

        public Project Update(Project entity)
        {
            return _context.Projects.Update(entity).Entity;
        }

        public void Delete(Project entity)
        {
            _context.Projects.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
