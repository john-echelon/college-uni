using CollegeUni.Api.Controllers;
using CollegeUni.Api.Test.Core;
using CollegeUni.Services.Models;
using CollegeUni.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Xunit;
namespace CollegeUni.Api.Test
{
    public class WhenUserRequestsLogin : TestBase
    {
        private Mock<IAuthService> mockAuthService;
        private Mock<ILogger<AccountController>> mockLogger;
        private AccountController controller;
        private LoginRequest model;
        private AuthServiceResult authServiceResult;
        protected override void TestSetup()
        {
            mockAuthService = new Mock<IAuthService>();
            mockLogger = new Mock<ILogger<AccountController>>();
            controller = new AccountController(mockAuthService.Object, mockLogger.Object);
            SetupData();
        }
        [Fact]
        public async Task ShouldReturnBadRequestObjectResultOnFailedSignIn()
        {
            // Arrange
            authServiceResult.UserSignIn = Microsoft.AspNetCore.Identity.SignInResult.Failed;
            mockAuthService.Setup(svc => svc.ValidateUser(model)).Returns(Task.FromResult<AuthServiceResult>(authServiceResult));
            mockAuthService.Setup(svc => svc.GetJwtSecurityToken(authServiceResult)).Returns(Task.FromResult<TokenResponse>(null));

            // Act
            var result = await controller.Token(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockAuthService.Verify(svc => svc.ValidateUser(model), Times.Once());
            mockAuthService.Verify(svc => svc.GetJwtSecurityToken(authServiceResult), Times.Never());
        }
        public async Task ShouldReturnBadRequestResultOnFailedTokenGeneration()
        {
            // Arrange
            mockAuthService.Setup(svc => svc.ValidateUser(model)).Returns(Task.FromResult<AuthServiceResult>(authServiceResult));
            mockAuthService.Setup(svc => svc.GetJwtSecurityToken(authServiceResult)).Returns(Task.FromResult<TokenResponse>(null));

            // Act
            var result = await controller.Token(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockAuthService.Verify(svc => svc.ValidateUser(model), Times.Once());
            mockAuthService.Verify(svc => svc.GetJwtSecurityToken(authServiceResult), Times.Once());
        }
        [Fact]
        public async Task ShouldReturnOkObjectResultWithTokenOnSuccessfulSignIn()
        {
            // Arrange
            // Setup the mock behavior of ValidateUser
            mockAuthService.Setup(svc => svc.ValidateUser(model)).Returns(Task.FromResult<AuthServiceResult>(authServiceResult));
            // Setup the mock behavior of GetJwtSecurityToken
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Im1hcmlseW40NDNAZXhhbXBsZS5jb20iLCJhZG1pbiI6dHJ1ZX0.";
            var tokenWithSignature = $"{expectedToken}HNQL2R8fer1A-88xrcW4VPvprtADDjznCvWCTkf7Q10";
            var securityToken = new JwtSecurityToken(tokenWithSignature);
            var tokenVm = new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = securityToken.ValidTo
            };
            mockAuthService.Setup(svc => svc.GetJwtSecurityToken(authServiceResult)).Returns(Task.FromResult<TokenResponse>(tokenVm));

            // Act
            var result = await controller.Token(model);

            // Assert
            // Verify contents of payload
            Assert.IsType<OkObjectResult>(result);
            var response = result as OkObjectResult;
            var actual = response.Value as TokenResponse;
            Assert.Equal(expectedToken, actual.Token);
            // Verify appropriate methods were called
            mockAuthService.Verify(svc => svc.ValidateUser(model), Times.Once());
            mockAuthService.Verify(svc => svc.GetJwtSecurityToken(authServiceResult), Times.Once());
        }

        private void SetupData()
        {
            model = new LoginRequest
            {
                Email = "marilyn443@example.com",
                Password = "EchoLimaBudgie10320"
            };

            authServiceResult = new AuthServiceResult
            {
                User = new Data.Entities.ApplicationUser
                {
                    Email = "marilyn443@example.com"
                },
                UserSignIn = Microsoft.AspNetCore.Identity.SignInResult.Success
            };

        }
    }
    public class WhenRequestToRegisterNewUser : TestBase
    {
        private Mock<IAuthService> mockAuthService;
        private Mock<ILogger<AccountController>> mockLogger;
        private AccountController controller;
        private RegisterRequest model;
        protected override void TestSetup()
        {
            mockAuthService = new Mock<IAuthService>();
            mockLogger = new Mock<ILogger<AccountController>>();
            controller = new AccountController(mockAuthService.Object, mockLogger.Object);
            SetupData();
        }

        [Fact]
        public async Task ShouldReturnBadRequestObjectResultOnUnsuccessfulRegistration()
        {
            // Setup RegisterUser
            var authServiceResult = new AuthServiceResult
            {
                User = new Data.Entities.ApplicationUser
                {
                    Email = "marilyn443@example.com"
                },
                UserIdentity = IdentityResult.Failed(
                    new IdentityError[] { new IdentityError { Code = "Identiy Error Code", Description = "Some Identity Error Description" } })
            };
            mockAuthService.Setup(svc => svc.RegisterUser(model)).Returns(Task.FromResult<AuthServiceResult>(authServiceResult));

            var result = await controller.Register(model);
            // Verify contents of payload
            Assert.IsType<BadRequestObjectResult>(result);
            mockAuthService.Verify(svc => svc.RegisterUser(model), Times.Once());
        }
        [Fact]
        public async Task ShouldReturnOkResultOnSuccessfulRegistration()
        {
            // Setup RegisterUser
            var authServiceResult = new AuthServiceResult
            {
                UserIdentity = IdentityResult.Success
            };
            mockAuthService.Setup(svc => svc.RegisterUser(model)).Returns(Task.FromResult<AuthServiceResult>(authServiceResult));

            var result = await controller.Register(model);
            // Verify contents of payload
            Assert.IsType<OkResult>(result);
            // Verify appropriate methods were called
            mockAuthService.Verify(svc => svc.RegisterUser(model), Times.Once());
        }
        private void SetupData()
        {
            model = new RegisterRequest
            {
                Email = "marilyn443@example.com",
                Password = "EchoLimaBudgie10320",
                ConfirmPassword = "EchoLimaBudgie10320"
            };
        }
    }
}