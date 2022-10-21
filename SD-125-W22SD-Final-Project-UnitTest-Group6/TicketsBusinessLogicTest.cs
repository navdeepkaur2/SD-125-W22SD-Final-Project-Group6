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
            mockTicketsRepository.Verify(x => x.Create(testTicket), Times.Once);
            mockTicketsRepository.Verify(x => x.Save(), Times.Once);
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
            mockTicketsRepository.Verify(x => x.Create(testTicket), Times.Never);
            mockTicketsRepository.Verify(x => x.Save(), Times.Never);
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
    }
}
