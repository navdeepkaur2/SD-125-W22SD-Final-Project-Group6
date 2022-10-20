using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.DAL;
using Microsoft.AspNetCore.Identity;

namespace SD_125_W22SD_Final_Project_UnitTest_Group6
{
    [TestClass]
    public class ProjectsBusinessLogicTest
    {
        [TestMethod]
        public async Task ShouldCreateNewProject()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "1",
                UserName = "User1"
            };
            var testAssignedUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "User2"
                }
            };
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };

            var testAddedProjects = new List<Project>();
            var testSavedProjects = new List<Project>();

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockUserManager
                .Setup(x => x.FindByIdAsync(It.IsIn<string>(testAssignedUsers.Select(u => u.Id))))
                .ReturnsAsync((string userId) =>
                {
                    return testAssignedUsers.Find(u => u.Id == userId)!;
                });
            mockProjectsRepository
               .Setup(x => x.Create(It.IsAny<Project>()))
               .Callback((Project project) =>
               {
                   testAddedProjects.Add(project);
               });
            mockProjectsRepository
               .Setup(x => x.Save())
               .Callback(() =>
               {
                   testSavedProjects.AddRange(testAddedProjects);
                   testAddedProjects.Clear();
               });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);
            
            // Act
            await projectsBusinessLogic.Create(testUser.Id, testProject, testAssignedUsers.Select(u => u.Id).ToList());

            // Assert
            Assert.AreEqual(1, testSavedProjects.Count());
            var resultProject = testSavedProjects.First();
            Assert.AreEqual(testProject.Id, resultProject.Id);
        }

        [TestMethod]
        public void ShouldReturnCorrectProjectWhenMatchingIdPassed()
        {
            var project = new Project
            {
                Id = 1,
                ProjectName = "Project1",
            };
            var projectsBusinessLogic = GetMockProjectsBusinessLogic(null, null, new List<Project> { project }, null, null);

            var result = projectsBusinessLogic.FindById(project.Id);

            Assert.AreEqual(project.Id, result?.Id);
        }

        [TestMethod]
        public void ShouldReturnNullWhenNoMatchingIdPassed()
        {
            var project = new Project
            {
                Id = 1,
                ProjectName = "Project1",
            };
            var projectsBusinessLogic = GetMockProjectsBusinessLogic(null, null, new List<Project> { project }, null, null);

            var result = projectsBusinessLogic.FindById(-1);

            Assert.IsNull(result);
        }

        private ProjectsBusinessLogic GetMockProjectsBusinessLogic(
            List<ApplicationUser>? users,
            Dictionary<string, string>? userRoles,
            List<Project>? projects,
            List<Ticket>? tickets,
            Mock<ApplicationDbContext>? mockDbContext
            )
        {
            users ??= new List<ApplicationUser>();
            userRoles ??= new Dictionary<string, string>();
            projects ??= new List<Project>();
            tickets ??= new List<Ticket>();

            var queryableProjects = projects.AsQueryable();
            var queryableTickets = tickets.AsQueryable();

            var mockProjectDbSet = new Mock<DbSet<Project>>();
            var mockTicketDbSet = new Mock<DbSet<Ticket>>();

            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.Provider).Returns(queryableProjects.Provider);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.Expression).Returns(queryableProjects.Expression);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.ElementType).Returns(queryableProjects.ElementType);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(x => x.GetEnumerator()).Returns(queryableProjects.GetEnumerator());
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.Provider).Returns(queryableTickets.Provider);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.Expression).Returns(queryableTickets.Expression);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.ElementType).Returns(queryableTickets.ElementType);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(x => x.GetEnumerator()).Returns(queryableTickets.GetEnumerator());

            mockDbContext ??= new Mock<ApplicationDbContext>();
            mockDbContext.Setup(x => x.Projects).Returns(mockProjectDbSet.Object);
            mockDbContext.Setup(x => x.Tickets).Returns(mockTicketDbSet.Object);

            var projectsBusinessLogic = new ProjectsBusinessLogic(
                FakeUserManager.GetFakeUserManager(users, userRoles),
                new ProjectsRepository(mockDbContext.Object),
                new TicketsRepository(mockDbContext.Object)
                );

            return projectsBusinessLogic;
        }
    }
}
