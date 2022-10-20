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
        public async Task ShouldThrowExceptionWhenCreatingProjectAndPassedUserIdNotFound()
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
                .Setup(x => x.FindByIdAsync(It.IsIn(testAssignedUsers.Select(u => u.Id))))
                .ReturnsAsync((string userId) =>
                {
                    return testAssignedUsers.Find(u => u.Id == userId)!;
                });
            mockUserManager
                .Setup(x => x.FindByIdAsync(It.Is((string userId) => userId != testUser.Id && !testAssignedUsers.Select(u => u.Id).Contains(userId))))
                .Returns(Task.FromResult<ApplicationUser?>(null));
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

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await projectsBusinessLogic.Create("3", testProject, testAssignedUsers.Select(u => u.Id).ToList());
            });
        }

        [TestMethod]
        public void ShouldReturnCorrectProjectWhenMatchingIdPassed()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.IsAny<int>()))
                .Returns(testProject);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultProject = projectsBusinessLogic.FindById(testProject.Id);

            // Assert
            Assert.AreEqual(testProject.Id, resultProject?.Id);
        }

        [TestMethod]
        public void ShouldReturnNullWhenNoMatchingIdPassed()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id != testProject.Id)))
                .Returns((Project?)null);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultProject = projectsBusinessLogic.FindById(-1);

            // Assert
            Assert.IsNull(resultProject);
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
