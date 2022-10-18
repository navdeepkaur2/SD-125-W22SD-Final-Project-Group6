using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class CreateTicketViewModel
    {
        public int SelectedProject { get; set; }
        public IEnumerable<SelectListItem> Projects { get; set; }
    }
}
