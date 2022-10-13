using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.DAL
{
    public class TicketsRepository : IRepository<Ticket>
    {
        private readonly ApplicationDbContext _context;

        public TicketsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Ticket entity)
        {
            _context.Tickets.Add(entity);
        }

        public Ticket? FindById(int id)
        {
            return _context.Tickets.First(t => t.Id == id);
        }

        public Ticket? Find(Func<Ticket, bool> predicate)
        {
            return _context.Tickets.First(predicate);
        }

        public ICollection<Ticket> FindList(Func<Ticket, bool> predicate)
        {
            return _context.Tickets.Where(predicate).ToList();
        }

        public ICollection<Ticket> GetAll()
        {
            return _context.Tickets.ToList();
        }

        public Ticket Update(Ticket entity)
        {
            return _context.Tickets.Update(entity).Entity;
        }

        public void Delete(Ticket entity)
        {
            _context.Tickets.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
