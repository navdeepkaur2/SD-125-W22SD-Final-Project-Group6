using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base()
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<UserProject> UserProjects { get; set; }
        public virtual DbSet<TicketWatcher> TicketWatchers { get; set; }
    }
}