using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketsBusinessLogic
    {
        private readonly TicketsRepository _ticketsRepository;

        public TicketsBusinessLogic(TicketsRepository ticketsRepository)
        {
            _ticketsRepository = ticketsRepository;
        }

        public Ticket? FindById(int id)
        {
            return _ticketsRepository.FindById(id);
        }

        public List<Ticket> GetAll()
        {
            return _ticketsRepository.GetAll().ToList();
        }
    }
}
