using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.DAL;

namespace SD_125_W22SD_Final_Project_UnitTest_Group6
{
    [TestClass]
    public class ProjectsBusinessLogicTest
    {
        private ProjectsBusinessLogic projectsBusinessLogic = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "User1",
                }
            };
            var userRoles = new Dictionary<string, string>
            {
                { users[0].Id, "Admin" }
            };
            var projects = new List<Project>
            {
                 new Project
                 {
                    Id = 1,
                    ProjectName = "Project1",
                 }
            };
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                }
            };

            var queryableProjects = projects.AsQueryable();
            var queryableTickets = tickets.AsQueryable();

            var mockProjectDbSet = new Mock<DbSet<Project>>();
            var mockTicketDbSet = new Mock<DbSet<Ticket>>();

            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.Provider).Returns(queryableProjects.Provider);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.Expression).Returns(queryableProjects.Expression);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.ElementType).Returns(queryableProjects.ElementType);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.GetEnumerator()).Returns(queryableProjects.GetEnumerator);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.Provider).Returns(queryableTickets.Provider);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.Expression).Returns(queryableTickets.Expression);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.ElementType).Returns(queryableTickets.ElementType);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.GetEnumerator()).Returns(queryableTickets.GetEnumerator);

            var mockDbContext = new Mock<ApplicationDbContext>();

            mockDbContext.Setup(x => x.Projects).Returns(mockProjectDbSet.Object);
            mockDbContext.Setup(x => x.Tickets).Returns(mockTicketDbSet.Object);

            projectsBusinessLogic = new ProjectsBusinessLogic(
                FakeUserManager.GetFakeUserManager(users, userRoles),
                new ProjectsRepository(mockDbContext.Object),
                new TicketsRepository(mockDbContext.Object)
                );
        }

        [TestMethod]
        public void ShouldReturnCorrectProjectWhenMatchingIdPassed()
        {
            var result = projectsBusinessLogic.FindById(1);

            Assert.AreEqual(1, result?.Id);
        }

        [TestMethod]
        public void ShouldReturnNullWhenNoMatchingIdPassed()
        {
            var result = projectsBusinessLogic.FindById(-1);

            Assert.IsNull(result);
        }
    }
}
