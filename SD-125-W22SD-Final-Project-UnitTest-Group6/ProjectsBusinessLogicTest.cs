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

            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Callback(() => { });

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

        [TestMethod]
        public void ShouldReturnListOfProjects()
        {
            // Arrange                      
            var testProjects = new List<Project>();
            for (var i = 1; i <= 10; i++)
            {
                testProjects.Add(
                        new Project
                        {
                            Id = i,
                            ProjectName = $"Project{i}"
                        }
                    );
            }

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindList(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(testProjects);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultProject = projectsBusinessLogic.FindByPage();

            // Assert
            Assert.AreEqual(testProjects.Count, resultProject.Count);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void ShouldThrowExceptionWhenPageIsZeroOrNegative(int page)
        {
            // Arrange                      
            var testProjects = new List<Project>();
            for (var i = 1; i <= 10; i++)
            {
                testProjects.Add(
                        new Project
                        {
                            Id = i,
                            ProjectName = $"Project{i}"
                        }
                    );
            }

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindList(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(testProjects);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                projectsBusinessLogic.FindByPage(page, 10);
            });
        }

        [TestMethod]
        public void ShouldDeleteProjectsAndTicketsAndAssignedUsers()
        {
            // Arrange
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };
            var testTickets = new List<Ticket>();
            for (var i = 1; i <= 5; i++)
            {
                var testTicket = new Ticket
                {
                    Id = i,
                    Title = $"Ticket{i}",
                    Body = $"Body{i}",
                    RequiredHours = 2,
                    TicketPriority = Ticket.Priority.Medium,
                    Completed = true
                };
                testTickets.Add(testTicket);
            }
            var testAssignedUsers = new List<ApplicationUser>();
            for (var i = 1; i <= 5; i++)
            {
                var testUser = new ApplicationUser
                {
                    Id = $"UserId{i}",
                    UserName = $"UserName{i}"
                };
                testAssignedUsers.Add(testUser);
            }
            var testUserProjects = new List<UserProject>();
            for (var i = 1; i <= 5; i++)
            {
                var testUserProject = new UserProject
                {
                    Id = i,
                    UserId = $"UserId{i}",
                    ProjectId = 1
                };
                testUserProjects.Add(testUserProject);
            }
            testProject.Tickets = testTickets;
            testProject.AssignedTo = testUserProjects;

            var ticketsReadyToDeleted = false;
            var assignedUsersReadyToDeleted = false;
            var projectReadyToDeleted = false;
            var ticketsDeleted = false;
            var assignedUsersDeleted = false;
            var projectDeleted = false;

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.IsAny<int>()))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.RemoveAssignedUser(It.IsAny<int>(), It.IsAny<string>()))
                .Callback((int projectId, string userId) =>
                {
                    assignedUsersReadyToDeleted = true;
                });
            mockProjectsRepository
               .Setup(x => x.Delete(It.IsAny<Project>()))
               .Callback((Project project) =>
               {
                   projectReadyToDeleted = true;
               });
            mockProjectsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (assignedUsersReadyToDeleted)
                    {
                        assignedUsersDeleted = true;
                    }
                    if (projectReadyToDeleted)
                    {
                        projectDeleted = true;
                    }
                });
            mockTicketsRepository
                .Setup(x => x.Delete(It.IsAny<Ticket>()))
                .Callback((Ticket ticket) =>
                {
                    ticketsReadyToDeleted = true;
                });
            mockTicketsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (ticketsReadyToDeleted)
                    {
                        ticketsDeleted = true;
                    }
                });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            projectsBusinessLogic.Delete(testProject.Id);

            // Assert
            Assert.IsTrue(ticketsDeleted);
            Assert.IsTrue(assignedUsersDeleted);
            Assert.IsTrue(projectDeleted);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenDeletingProjectAndNotFound()
        {
            // Arrange
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };
            var testTickets = new List<Ticket>();
            for (var i = 1; i <= 5; i++)
            {
                var testTicket = new Ticket
                {
                    Id = i,
                    Title = $"Ticket{i}",
                    Body = $"Body{i}",
                    RequiredHours = 2,
                    TicketPriority = Ticket.Priority.Medium,
                    Completed = true
                };
                testTickets.Add(testTicket);
            }
            var testAssignedUsers = new List<ApplicationUser>();
            for (var i = 1; i <= 5; i++)
            {
                var testUser = new ApplicationUser
                {
                    Id = $"UserId{i}",
                    UserName = $"UserName{i}"
                };
                testAssignedUsers.Add(testUser);
            }
            var testUserProjects = new List<UserProject>();
            for (var i = 1; i <= 5; i++)
            {
                var testUserProject = new UserProject
                {
                    Id = i,
                    UserId = $"UserId{i}",
                    ProjectId = 1
                };
                testUserProjects.Add(testUserProject);
            }
            testProject.Tickets = testTickets;
            testProject.AssignedTo = testUserProjects;

            var ticketsReadyToDeleted = false;
            var assignedUsersReadyToDeleted = false;
            var projectReadyToDeleted = false;
            var ticketsDeleted = false;
            var assignedUsersDeleted = false;
            var projectDeleted = false;

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.RemoveAssignedUser(It.IsAny<int>(), It.IsAny<string>()))
                .Callback((int projectId, string userId) =>
                {
                    assignedUsersReadyToDeleted = true;
                });
            mockProjectsRepository
               .Setup(x => x.Delete(It.IsAny<Project>()))
               .Callback((Project project) =>
               {
                   projectReadyToDeleted = true;
               });
            mockProjectsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (assignedUsersReadyToDeleted)
                    {
                        assignedUsersDeleted = true;
                    }
                    if (projectReadyToDeleted)
                    {
                        projectDeleted = true;
                    }
                });
            mockTicketsRepository
                .Setup(x => x.Delete(It.IsAny<Ticket>()))
                .Callback((Ticket ticket) =>
                {
                    ticketsReadyToDeleted = true;
                });
            mockTicketsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (ticketsReadyToDeleted)
                    {
                        ticketsDeleted = true;
                    }
                });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                projectsBusinessLogic.Delete(-1);
            });
        }

        [TestMethod]
        public void ShouldReturnProjectCount()
        {
            // Arrange                      
            var testCount = 10;

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.Count())
                .Returns(testCount);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultCount = projectsBusinessLogic.Count();

            // Assert
            Assert.AreEqual(testCount, resultCount);
        }

        [TestMethod]
        public void ShouldReturnTrueWhenProjectExists()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultExists = projectsBusinessLogic.Exists(testProject.Id);

            // Assert
            Assert.IsTrue(resultExists);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenProjectDoesNotExist()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            var resultExists = projectsBusinessLogic.Exists(2);

            // Assert
            Assert.IsFalse(resultExists);
        }

        [TestMethod]
        public void ShouldRenameProject()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };
            var testProjectReadyToUpdate = false;
            var testProjectUpdated = false;
            var testProjectNewName = "Project2";

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.Update(It.Is((Project project) => project.Id == testProject.Id)))
                .Callback((Project project) =>
                {
                    testProjectReadyToUpdate = true;
                });
            mockProjectsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (testProjectReadyToUpdate)
                    {
                        testProjectUpdated = true;
                        testProject.ProjectName = testProjectNewName;
                    }
                });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            projectsBusinessLogic.RenameProject(testProject.Id, testProjectNewName);

            // Assert
            Assert.IsTrue(testProjectUpdated);
            Assert.AreEqual(testProjectNewName, testProject.ProjectName);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenRenamingAndProjectNotFound()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1,
                ProjectName = "Project1"
            };
            var testProjectReadyToUpdate = false;
            var testProjectUpdated = false;
            var testProjectNewName = "Project2";

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.Update(It.Is((Project project) => project.Id == testProject.Id)))
                .Callback((Project project) =>
                {
                    testProjectReadyToUpdate = true;
                });
            mockProjectsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (testProjectReadyToUpdate)
                    {
                        testProjectUpdated = true;
                        testProject.ProjectName = testProjectNewName;
                    }
                });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                projectsBusinessLogic.RenameProject(2, testProjectNewName);
            });
            Assert.IsFalse(testProjectUpdated);
        }

        [TestMethod]
        public async Task ShouldAddAssignedUsersToProject()
        {
            // Arrange                      
            var testProject = new Project
            {
                Id = 1,
            };
            var testProjectReadyToUpdate = false;
            var testProjectUpdated = false;
            var testAssignedUsers = new List<ApplicationUser>();
            for (var i = 1; i <= 3; i++)
            {
                var testAssignedUser = new ApplicationUser
                {
                    Id = $"UserId{i}",
                    UserName = $"UserName{i}"
                };
                testAssignedUsers.Add(testAssignedUser);
            }

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();

            mockProjectsRepository
                .Setup(x => x.FindById(It.Is((int id) => id == testProject.Id)))
                .Returns(testProject);
            mockProjectsRepository
                .Setup(x => x.Update(It.Is((Project project) => project.Id == testProject.Id)))
                .Callback((Project project) =>
                {
                    testProjectReadyToUpdate = true;
                });
            mockProjectsRepository
                .Setup(x => x.Save())
                .Callback(() =>
                {
                    if (testProjectReadyToUpdate)
                    {
                        testProjectUpdated = true;
                    }
                });
            mockUserManager
                .Setup(x => x.FindByIdAsync(It.IsIn(testAssignedUsers.Select(u => u.Id))))
                .ReturnsAsync((string userId) =>
                {
                    return testAssignedUsers.First(u => u.Id == userId);
                });

            var projectsBusinessLogic = new ProjectsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object);

            // Act
            await projectsBusinessLogic.AddAssignedUsers(testProject.Id, testAssignedUsers.Select(u => u.Id).ToList());

            // Assert
            Assert.IsTrue(testProjectUpdated);
            foreach (var resultUserProject in testProject.AssignedTo)
            {
                Assert.IsTrue(testAssignedUsers.Select(u => u.Id).Contains(resultUserProject.UserId));
            }
        }
    }
}
