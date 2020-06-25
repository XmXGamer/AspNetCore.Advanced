using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using PermissionAuthorization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public PermissionsMiddlewareTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockRequestDelegate = mockRepository.Create<RequestDelegate>();
        }

        private PermissionsMiddleware CreatePermissionsMiddleware()
        {
            return CreatePermissionsMiddleware(Enumerable.Empty<IPermissionSource>());
        }

        private PermissionsMiddleware CreatePermissionsMiddleware(IEnumerable<IPermissionSource> permissionSources)
        {
            mockRequestDelegate.Setup(c => c.Invoke(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            return new PermissionsMiddleware(
                mockRequestDelegate.Object,
                permissionSources);
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

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            mockRequestDelegate.Verify(x => x.Invoke(context), Times.Once);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_IfPermissionSourcesIsNull()
        {
            // Arrange
            var permissionsMiddleware = CreatePermissionsMiddleware(null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], "TestAuthentication"));
            var context = new DefaultHttpContext()
            {
                User = user
            };

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

        [Fact]
        public async Task Invoke_UserShouldHavePermission_IfPermissionSourcesIsSet()
        {
            // Arrange
            var permissionSource = mockRepository.Create<IPermissionSource>();
            permissionSource.Setup(c => c.GetPermissions(It.IsAny<HttpContext>())).Returns((new List<string>() { "test" }).ToImmutableList());
            var permissionsMiddleware = CreatePermissionsMiddleware(new List<IPermissionSource>() { permissionSource.Object });
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], "TestAuthentication"));
            var context = new DefaultHttpContext()
            {
                User = user
            };

            // Act
            await permissionsMiddleware.InvokeAsync(
                context);

            // Assert
            user.Claims.Should().Contain(x => x.Type == "permissions" && x.Value == "test");
            mockRepository.VerifyAll();
        }
    }
}