using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Users.Brokers;
using Jellyfin.Server.Implementations.Users.Exceptions;
using Jellyfin.Server.Implementations.Users.Services;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task AddUserAsync_ValidUser_CallsBrokerInsertAndReturnsSameUser()
        {
            // given
            User inputUser = CreateRandomUser();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.InsertUserAsync(It.IsAny<User>()))
                .Returns(new ValueTask<User>(inputUser));
            var service = new UserService(brokerMock.Object);

            // when
            User actualUser = await service.AddUserAsync(inputUser);

            // then
            Assert.Same(inputUser, actualUser);
            brokerMock.Verify(b => b.InsertUserAsync(inputUser), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_NullUser_ThrowsInvalidUserException()
        {
            // given
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.AddUserAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_EmptyUsername_ThrowsInvalidUserException()
        {
            // given
            User invalidUser = CreateRandomUser();
            invalidUser.Username = string.Empty;
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.AddUserAsync(invalidUser).AsTask());
            brokerMock.Verify(b => b.InsertUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_EmptyAuthenticationProviderId_ThrowsInvalidUserException()
        {
            // given
            User invalidUser = CreateRandomUser();
            invalidUser.AuthenticationProviderId = string.Empty;
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.AddUserAsync(invalidUser).AsTask());
            brokerMock.Verify(b => b.InsertUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_EmptyPasswordResetProviderId_ThrowsInvalidUserException()
        {
            // given
            User invalidUser = CreateRandomUser();
            invalidUser.PasswordResetProviderId = string.Empty;
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.AddUserAsync(invalidUser).AsTask());
            brokerMock.Verify(b => b.InsertUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_EmptyId_ThrowsInvalidUserException()
        {
            // given
            User invalidUser = CreateRandomUser();
            invalidUser.Id = Guid.Empty;
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.AddUserAsync(invalidUser).AsTask());
            brokerMock.Verify(b => b.InsertUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllUsersAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<User> expectedUsers = CreateRandomUsers();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.SelectAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(expectedUsers));
            var service = new UserService(brokerMock.Object);

            // when
            IReadOnlyList<User> actualUsers = await service.RetrieveAllUsersAsync();

            // then
            Assert.Equal(expectedUsers, actualUsers);
            brokerMock.Verify(b => b.SelectAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveUserByIdAsync_EmptyId_ThrowsInvalidUserException()
        {
            // given
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.RetrieveUserByIdAsync(Guid.Empty).AsTask());
            brokerMock.Verify(b => b.SelectUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveUserByIdAsync_NotFound_ThrowsUserNotFoundException()
        {
            // given
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.SelectUserByIdAsync(It.IsAny<Guid>()))
                .Returns(new ValueTask<User?>((User?)null));
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<UserNotFoundException>(
                () => service.RetrieveUserByIdAsync(Guid.NewGuid()).AsTask());
        }

        [Fact]
        public async Task RetrieveUserByIdAsync_Found_ReturnsUser()
        {
            // given
            User expectedUser = CreateRandomUser();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.SelectUserByIdAsync(It.IsAny<Guid>()))
                .Returns(new ValueTask<User?>(expectedUser));
            var service = new UserService(brokerMock.Object);

            // when
            User actualUser = await service.RetrieveUserByIdAsync(Guid.NewGuid());

            // then
            Assert.Same(expectedUser, actualUser);
        }

        [Fact]
        public async Task ModifyUserAsync_NullUser_ThrowsInvalidUserException()
        {
            // given
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.ModifyUserAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task ModifyUserAsync_ValidUser_CallsBrokerUpdate()
        {
            // given
            User inputUser = CreateRandomUser();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.UpdateUserAsync(It.IsAny<User>()))
                .Returns(new ValueTask<User>(inputUser));
            var service = new UserService(brokerMock.Object);

            // when
            User actualUser = await service.ModifyUserAsync(inputUser);

            // then
            Assert.Same(inputUser, actualUser);
            brokerMock.Verify(b => b.UpdateUserAsync(inputUser), Times.Once);
        }

        [Fact]
        public async Task ModifyUserAsync_EmptyUsername_ThrowsInvalidUserException()
        {
            // given
            User invalidUser = CreateRandomUser();
            invalidUser.Username = string.Empty;
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.ModifyUserAsync(invalidUser).AsTask());
            brokerMock.Verify(b => b.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RemoveUserAsync_NullUser_ThrowsInvalidUserException()
        {
            // given
            var brokerMock = new Mock<IUserBroker>();
            var service = new UserService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidUserException>(
                () => service.RemoveUserAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RemoveUserAsync_ValidUser_CallsBrokerDelete()
        {
            // given
            User inputUser = CreateRandomUser();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.DeleteUserAsync(It.IsAny<User>()))
                .Returns(new ValueTask<User>(inputUser));
            var service = new UserService(brokerMock.Object);

            // when
            User actualUser = await service.RemoveUserAsync(inputUser);

            // then
            Assert.Same(inputUser, actualUser);
            brokerMock.Verify(b => b.DeleteUserAsync(inputUser), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_BrokerThrows_IsWrappedAsUserServiceException()
        {
            // given
            User inputUser = CreateRandomUser();
            var brokerMock = new Mock<IUserBroker>();
            brokerMock.Setup(b => b.InsertUserAsync(It.IsAny<User>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new UserService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<UserServiceException>(
                () => service.AddUserAsync(inputUser).AsTask());
            Assert.IsType<FailedUserServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertUserAsync(inputUser), Times.Once);
        }
    }
}
