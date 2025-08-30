using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Holocron.App.Api.Controllers;
using Holocron.App.Api.Data;
using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Interfaces;
using Holocron.App.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Holocron.Tests.Api
{
    public class CommentsControllerTests
    {
        private readonly Mock<DataContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IValidator<NewCommentRequest>> _mockValidator;
        private readonly Mock<IIdentityProvider> _mockIdentityProvider;
        private readonly DbContextOptions<DataContext> _dbOptions;

        public CommentsControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CommentsTestDb")
                .Options;
            _mockContext = new Mock<DataContext>(_dbOptions);
            _mockMapper = new Mock<IMapper>();
            _mockValidator = new Mock<IValidator<NewCommentRequest>>();
            _mockIdentityProvider = new Mock<IIdentityProvider>();
        }

        private DataContext CreateInMemoryContext()
        {
            var context = new DataContext(_dbOptions, _mockIdentityProvider.Object);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetComments_ReturnsOkWithComments()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Comments.Add(new CommentEntity { Id = Guid.NewGuid(), Name = "test", TenantId = "user1", Review = "Hello" });
            context.SaveChanges();

            _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<CommentEntity>()))
                .Returns((CommentEntity ce) => new Comment { Id = ce.Id, Name = ce.Name, Review = ce.Review });

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.GetComments("test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var comments = Assert.IsAssignableFrom<IEnumerable<Comment>>(okResult.Value);
            Assert.Single(comments);
            Assert.Equal("test", comments.First().Name);
        }

        [Fact]
        public async Task CreateComment_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var request = new NewCommentRequest { Name = "test", Review = "" };
            _mockValidator.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Content", "Required") }));

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.CreateComment(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateComment_ValidModel_ReturnsCreatedAtAction()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var request = new NewCommentRequest { Name = "test", Review = "Nice!" };
            var entity = new CommentEntity { TenantId = "", Name = "test", Review = "Nice!" };
            var comment = new Comment { Name = "test", Review = "Nice!" };

            _mockValidator.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());
            _mockMapper.Setup(m => m.Map<CommentEntity>(request)).Returns(entity);
            _mockMapper.Setup(m => m.Map<Comment>(entity)).Returns(comment);

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.CreateComment(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetComments), createdResult.ActionName);
            Assert.Equal(comment, createdResult.Value);
        }

        [Fact]
        public async Task HasUserCommented_UserNotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            var context = CreateInMemoryContext();
            _mockIdentityProvider.Setup(i => i.GetCurrentUserId()).Returns((string?)null);

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.HasUserCommented("test");

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task HasUserCommented_UserHasCommented_ReturnsOkTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Comments.Add(new CommentEntity { Name = "test", TenantId = "user1", Review = "Hello" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(i => i.GetCurrentUserId()).Returns("user1");

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.HasUserCommented("test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(okResult.Value is bool b && b);
        }

        [Fact]
        public async Task HasUserCommented_UserHasNotCommented_ReturnsOkFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Comments.Add(new CommentEntity { Name = "test", TenantId = "user2", Review = "Hello" });
            context.SaveChanges();

            _mockIdentityProvider.Setup(i => i.GetCurrentUserId()).Returns("user1");

            var controller = new CommentsController(context, _mockMapper.Object, _mockValidator.Object, _mockIdentityProvider.Object);

            // Act
            var result = await controller.HasUserCommented("test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False(okResult.Value is bool b && b);
        }
    }
}