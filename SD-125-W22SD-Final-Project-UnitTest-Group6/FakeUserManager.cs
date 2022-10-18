using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_125_W22SD_UnitTest
{
    public class FakeUserManager : UserManager<ApplicationUser>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
        { }

        public static UserManager<ApplicationUser> GetFakeUserManager(List<ApplicationUser> users, Dictionary<string, string>? userRoles)
        {
            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.Users).Returns(users.AsQueryable());
            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string id) => users.Find(u => u.Id == id));
            fakeUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((string name) => users.Find(u => u.UserName == name));
            fakeUserManager.Setup(x => x.GetUsersInRoleAsync(It.IsAny<string>())).ReturnsAsync((string role) => users.Where(u => userRoles.Where(ur => ur.Value == role).Select(p => p.Key).Contains(u.Id)).ToList());
            fakeUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            return fakeUserManager.Object;
        }
    }
}
