using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class CommentsRepository : IRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentsRepository() { }

        public CommentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual void Create(Comment entity)
        {
            _context.Add(entity);
        }

        public virtual Comment? Find(Func<Comment, bool> predicate)
        {
            return _context.Comments.FirstOrDefault(predicate);
        }

        public virtual Comment? FindById(int id)
        {
            return _context.Comments.FirstOrDefault(c => c.Id == id);
        }

        public virtual ICollection<Comment> FindList(Func<Comment, bool> predicate)
        {
            return _context.Comments.Where(predicate).ToList();
        }

        public virtual ICollection<Comment> GetAll()
        {
            return _context.Comments.ToList();
        }

        public virtual Comment Update(Comment entity)
        {
            return _context.Comments.Update(entity).Entity;
        }

        public virtual void Delete(Comment entity)
        {
            _context.Remove(entity);
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }
    }
}
