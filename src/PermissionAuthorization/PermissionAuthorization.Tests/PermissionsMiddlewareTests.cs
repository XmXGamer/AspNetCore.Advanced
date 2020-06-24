using Microsoft.AspNetCore.Http;
using Moq;
using PermissionAuthorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Advanced.PermissionAuthorization.Tests
{
    public class PermissionsMiddlewareTests
    {
        private MockRepository mockRepository;

        private Mock<RequestDelegate> mockRequestDelegate;
        private Mock<IEnumerable<IPermissionSource>> mockEnumerable;

        public PermissionsMiddlewareTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockRequestDelegate = mockRepository.Create<RequestDelegate>();
            mockEnumerable = mockRepository.Create<IEnumerable<IPermissionSource>>();
        }

        private PermissionsMiddleware CreatePermissionsMiddleware()
        {
            mockRequestDelegate.Setup(c => c.Invoke(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            return new PermissionsMiddleware(
                mockRequestDelegate.Object,
                mockEnumerable.Object);
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_IfContextIsNull()
        {
            // Arrange
            var permissionsMiddleware = CreatePermissionsMiddleware();
            HttpContext context = null;

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            mockRequestDelegate.Verify(x => x.Invoke(context), Times.Once);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_IfUserIsNull()
        {
            // Arrange
            var permissionsMiddleware = CreatePermissionsMiddleware();
            HttpContext context = new DefaultHttpContext();
            context.User = null;

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            mockRequestDelegate.Verify(x => x.Invoke(context), Times.Once);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_IfUserIsNotAuthenticated()
        {
            // Arrange
            var permissionsMiddleware = CreatePermissionsMiddleware();
            HttpContext context = new DefaultHttpContext();
            var identity = new Mock<IIdentity>();
            identity.SetupGet(i => i.IsAuthenticated).Returns(true);
            context.User = new ClaimsPrincipal(identity.Object);

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            mockRequestDelegate.Verify(x => x.Invoke(context), Times.Once);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_IfPermissionSourcesIsEmpty()
        {
            // Arrange
            var permissionsMiddleware = CreatePermissionsMiddleware();
            HttpContext context = new DefaultHttpContext();
            var identity = new Mock<IIdentity>();
            identity.SetupGet(i => i.IsAuthenticated).Returns(true);
            context.User = new ClaimsPrincipal(identity.Object);

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            mockRequestDelegate.Verify(x => x.Invoke(context), Times.Once);
            mockRepository.VerifyAll();
        }
    }
}