using Holocron.App.Api.Controllers;
using Holocron.App.Api.Data;
using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Holocron.Tests.Api
{
    public class LikesControllerTests
    {
        private readonly Mock<DataContext> _mockContext;
        private readonly Mock<ILogger<LikesController>> _mockLogger;
        private readonly Mock<IIdentityProvider> _mockIdentityProvider;
        private readonly DbSet<LikeEntity>? _mockDbSet;

        public LikesControllerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockIdentityProvider = new Mock<IIdentityProvider>();
            var context = new DataContext(options, _mockIdentityProvider.Object);
            _mockContext = new Mock<DataContext>(options) { CallBase = true };
            _mockLogger = new Mock<ILogger<LikesController>>();
        }

        private LikesController CreateController(DataContext? context = null)
        {
            return new LikesController(
                context ?? _mockContext.Object,
                _mockLogger.Object,
                _mockIdentityProvider.Object
            );
        }

        [Fact]
        public async Task GetLikesForPerson_ReturnsOk_WithCount()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "GetLikesForPerson")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object); ;
            context.Likes.Add(new LikeEntity("Luke") { TenantId = "user1" });
            context.Likes.Add(new LikeEntity("Luke") { TenantId = "user2" });
            context.SaveChanges();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetLikesForPerson("Luke");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(2, okResult.Value);
        }

        [Fact]
        public async Task HasUserLiked_ReturnsOk_TrueIfLiked()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "HasUserLikedTrue")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object);
            context.Likes.Add(new LikeEntity("Leia") { TenantId = "user1" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.HasUserLiked("Leia");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(okResult.Value is bool value && value);
        }

        [Fact]
        public async Task HasUserLiked_ReturnsOk_FalseIfNotLiked()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "HasUserLikedFalse")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object); ;
            context.Likes.Add(new LikeEntity("Leia") { TenantId = "user2" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.HasUserLiked("Leia");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False(okResult.Value is bool value && value);
        }

        [Fact]
        public async Task PostNewLike_AddsLike_WhenNotAlreadyLiked()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PostNewLike")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object); ;

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.PostNewLike("Han");

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Single(context.Likes);
            var like = await context.Likes.FirstAsync();
            Assert.Equal("Han", like.Name);
        }

        [Fact]
        public async Task PostNewLike_ReturnsBadRequest_IfAlreadyLiked()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PostNewLikeBadRequest")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object);
            context.Likes.Add(new LikeEntity("Han") { TenantId = "user1" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.PostNewLike("Han");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You have already liked this person.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteLike_RemovesLike_IfExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "DeleteLike")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object);
            context.Likes.Add(new LikeEntity("Chewie") { TenantId = "user1" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.DeleteLike("Chewie");

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Likes);
        }

        [Fact]
        public async Task DeleteLike_ReturnsNotFound_IfLikeDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "DeleteLikeNotFound")
                .Options;
            using var context = new DataContext(options, _mockIdentityProvider.Object);

            _mockIdentityProvider.Setup(x => x.GetCurrentUserId()).Returns("user1");
            var controller = CreateController(context);

            // Act
            var result = await controller.DeleteLike("Chewie");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("You have not liked this person.", notFound.Value);
        }
    }
}