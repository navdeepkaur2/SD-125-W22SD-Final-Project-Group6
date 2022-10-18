using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_125_W22SD_UnitTest
{
    [TestClass]
    public class UserBusinessLogicTest
    {
        [TestMethod]
        public async Task ShouldReturnCorrectUserWhenMatchingIdPassed()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "user1@jello.com"
            };
            var userManager = FakeUserManager.GetFakeUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var result = await userBusinessLogic.FindById(user.Id);

            Assert.AreEqual(user.Id, result.Id);
        }

        [TestMethod]
        public async Task ShouldReturnNullWhenNoMatchingIdPassed()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                Id = "UserId1",
                Email = "user1@jello.com"
            };
            var userManager = FakeUserManager.GetFakeUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var result = await userBusinessLogic.FindById("UserId2");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ShouldReturnCorrectUserWhenMatchingNamePassed()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                NormalizedUserName = "USER1",
                Id = "UserId1",
                Email = "user1@jello.com"
            };
            var userManager = FakeUserManager.GetFakeUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var result = await userBusinessLogic.FindByName(user.UserName);

            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.UserName, result.UserName);
        }

        [TestMethod]
        public async Task ShouldReturnNullWhenNoMatchingNamePassed()
        {
            var user = new ApplicationUser
            {
                UserName = "User1",
                NormalizedUserName = "USER1",
                Id = "UserId1",
                Email = "user1@jello.com"
            };
            var userManager = FakeUserManager.GetFakeUserManager(new List<ApplicationUser> { user }, null);
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var result = await userBusinessLogic.FindByName("User2");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ShouldReturnCorrectUsersWhenMatchingRolePassed()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "User1",
                    NormalizedUserName = "USER1",
                    Id = "UserId1",
                },
                new ApplicationUser
                {
                    UserName = "User2",
                    NormalizedUserName = "USER2",
                    Id = "UserId2",
                },
                new ApplicationUser
                {
                    UserName = "User3",
                    NormalizedUserName = "USER3",
                    Id = "UserId3",
                },
            };
            var userRole = "Developer";
            var userManager = FakeUserManager.GetFakeUserManager(
                users,
                new Dictionary<string, string> {
                    { users[0].Id, userRole },
                    { users[1].Id, userRole },
                    { users[2].Id, "ProjectManager" },
                }
                );
            var userBusinessLogic = new UserBusinessLogic(userManager);

            var result = await userBusinessLogic.GetUsersByRole(userRole);

            Assert.IsTrue(result.Select(user => user.Id).Contains(users[0].Id));
            Assert.IsTrue(result.Select(user => user.Id).Contains(users[1].Id));
        }
    }
}
