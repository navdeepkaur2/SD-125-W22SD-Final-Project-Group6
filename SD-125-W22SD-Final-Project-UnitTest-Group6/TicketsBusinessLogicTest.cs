using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.DAL;
using SD_340_W22SD_Final_Project_Group6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD_125_W22SD_Final_Project_UnitTest_Group6
{
    [TestClass]
    public class TicketsBusinessLogicTest
    {
        [TestMethod]
        public async Task ShouldCreateTicket()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testProject = new Project
            {
                Id = 1,
            };
            var testTicket = new Ticket
            {
                Id = 1,
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockProjectsRepository
                .Setup(x => x.FindById(testProject.Id))
                .Returns(testProject);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            await ticketsBusinessLogic.Create(testTicket, testProject.Id, testUser.Id);

            // Assert
            Assert.AreEqual(testProject.Id, testTicket.Project?.Id);
            Assert.AreEqual(testUser.Id, testTicket.Owner?.Id);
            mockTicketsRepository.Verify(x => x.Create(testTicket), Times.Once());
            mockTicketsRepository.Verify(x => x.Save(), Times.Once());
        }

        [TestMethod]
        public async Task ShouldThrowExceptionWhenCreatingTicketAndProjectNotFound()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testProject = new Project
            {
                Id = 1,
            };
            var testTicket = new Ticket
            {
                Id = 1,
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockProjectsRepository
                .Setup(x => x.FindById(testProject.Id))
                .Returns(testProject);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await ticketsBusinessLogic.Create(testTicket, 2, testUser.Id);
            });
            mockTicketsRepository.Verify(x => x.Create(testTicket), Times.Never());
            mockTicketsRepository.Verify(x => x.Save(), Times.Never());
        }

        [TestMethod]
        public void ShouldReturnTicketById()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1,
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultTicket = ticketsBusinessLogic.FindById(testTicket.Id);

            // Assert
            Assert.AreEqual(testTicket.Id, resultTicket?.Id);
        }

        [TestMethod]
        public void ShouldReturnNullWhenFindingTicketByIdAndTicketNotFound()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1,
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultTicket = ticketsBusinessLogic.FindById(2);

            // Assert
            Assert.IsNull(resultTicket);
        }

        [TestMethod]
        public void ShouldReturnAllTickets()
        {
            // Arrange            
            var testTickets = new List<Ticket>();
            for (var i = 1; i <= 5; i++)
            {
                var testTicket = new Ticket
                {
                    Id = i
                };
                testTickets.Add(testTicket);
            }

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.GetAll())
                .Returns(testTickets);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultTickets = ticketsBusinessLogic.GetAll();

            // Assert
            Assert.AreEqual(testTickets.Count, resultTickets.Count);
        }

        [TestMethod]
        public void ShouldReturnEmptyListWhenGettingAllTicketsButThereIsNoTicket()
        {
            // Arrange            
            var testTickets = new List<Ticket>();

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.GetAll())
                .Returns(testTickets);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultTickets = ticketsBusinessLogic.GetAll();

            // Assert
            Assert.AreEqual(0, resultTickets.Count);
        }

        [TestMethod]
        public void ShouldUpdateTicket()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            ticketsBusinessLogic.Update(testTicket);

            // Assert
            mockTicketsRepository.Verify(x => x.Update(testTicket), Times.Once());
            mockTicketsRepository.Verify(x => x.Save(), Times.Once());
        }


        [TestMethod]
        public void ShouldDeleteTicket()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            ticketsBusinessLogic.Delete(testTicket.Id);

            // Assert
            mockTicketsRepository.Verify(x => x.Delete(It.Is((Ticket ticket) => ticket.Id == testTicket.Id)), Times.Once());
            mockTicketsRepository.Verify(x => x.Save(), Times.Once());
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenDeletingAndTicketNotFound()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                ticketsBusinessLogic.Delete(2);
            });
            mockTicketsRepository.Verify(x => x.Delete(It.Is((Ticket ticket) => ticket.Id == testTicket.Id)), Times.Never());
            mockTicketsRepository.Verify(x => x.Save(), Times.Never());
        }

        [TestMethod]
        public void ShouldReturnTrueWhenTicketExists()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultExists = ticketsBusinessLogic.Exists(testTicket.Id);

            // Assert
            Assert.IsTrue(resultExists);
        }

        [TestMethod]
        public void ShouldReturnFalseWhenTicketDoesNotExist()
        {
            // Arrange            
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            var resultExists = ticketsBusinessLogic.Exists(2);

            // Assert
            Assert.IsFalse(resultExists);
        }


        [TestMethod]
        public async Task ShouldAddComment()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1
            };
            var testDescription = "Description";

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            await ticketsBusinessLogic.AddComment(testUser.Id, testTicket.Id, testDescription);

            // Assert
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.CreatedBy.Id == testUser.Id)), Times.Once());
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.Description == testDescription)), Times.Once());
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.Ticket.Id == testTicket.Id)), Times.Once());
            mockCommentsRepository.Verify(x => x.Save(), Times.Once());
        }


        [TestMethod]
        public async Task ShouldThrowExceptionWhenAddingCommentAndTicketNotFound()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1
            };
            var testDescription = "Description";

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await ticketsBusinessLogic.AddComment(testUser.Id, 2, testDescription);
            });
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.CreatedBy.Id == testUser.Id)), Times.Never());
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.Description == testDescription)), Times.Never());
            mockCommentsRepository.Verify(x => x.Create(It.Is((Comment comment) => comment.Ticket.Id == testTicket.Id)), Times.Never());
            mockCommentsRepository.Verify(x => x.Save(), Times.Never());
        }

        [TestMethod]
        public async Task ShouldAddUserToWatchers()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            await ticketsBusinessLogic.AddToWatchers(testUser.Id, testTicket.Id);

            // Assert
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.Count == 1)), Times.Once());
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.First().Ticket.Id == testTicket.Id)), Times.Once());
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.First().Watcher.Id == testUser.Id)), Times.Once());
            mockTicketsRepository.Verify(x => x.Save(), Times.Once());
        }

        [TestMethod]
        public async Task ShouldThrowExceptionWhenAddingUserToWatchersAndTicketNotFound()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await ticketsBusinessLogic.AddToWatchers(testUser.Id, 2);
            });
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.Count == 1)), Times.Never());
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.First().Ticket.Id == testTicket.Id)), Times.Never());
            mockTicketsRepository.Verify(x => x.Update(It.Is((Ticket ticket) => ticket.TicketWatchers.First().Watcher.Id == testUser.Id)), Times.Never());
            mockTicketsRepository.Verify(x => x.Save(), Times.Never());
        }

        [TestMethod]
        public async Task ShouldRemoveUserFromWatchers()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1,
                TicketWatchers = new List<TicketWatcher>
                {
                    new TicketWatcher
                    {
                        Id=  1,
                        Watcher = testUser
                    }
                }
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act
            await ticketsBusinessLogic.RemoveFromWatchers(testUser.Id, testTicket.Id);

            // Assert
            mockTicketsRepository.Verify(x => x.RemoveWatcher(It.Is((TicketWatcher tw) => tw.Watcher.Id == testUser.Id)), Times.Once());
            mockTicketsRepository.Verify(x => x.Save(), Times.Once());
        }

        [TestMethod]
        public async Task ShouldThrowExceptionWhenRemovingUserFromWatchersAndTicketNotFound()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "UserId1"
            };
            var testTicket = new Ticket
            {
                Id = 1,
                TicketWatchers = new List<TicketWatcher>
                {
                    new TicketWatcher
                    {
                        Id=  1,
                        Watcher = testUser
                    }
                }
            };

            var mockUserManager = new Mock<FakeUserManager>();
            var mockProjectsRepository = new Mock<ProjectsRepository>();
            var mockTicketsRepository = new Mock<TicketsRepository>();
            var mockCommentsRepository = new Mock<CommentsRepository>();

            mockUserManager
                .Setup(x => x.FindByIdAsync(testUser.Id))
                .ReturnsAsync(testUser);
            mockTicketsRepository
                .Setup(x => x.FindById(testTicket.Id))
                .Returns(testTicket);

            var ticketsBusinessLogic = new TicketsBusinessLogic(mockUserManager.Object, mockProjectsRepository.Object, mockTicketsRepository.Object, mockCommentsRepository.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await ticketsBusinessLogic.RemoveFromWatchers(testUser.Id, 2);
            });
            mockTicketsRepository.Verify(x => x.RemoveWatcher(It.Is((TicketWatcher tw) => tw.Watcher.Id == testUser.Id)), Times.Never());
            mockTicketsRepository.Verify(x => x.Save(), Times.Never());
        }
    }
}
