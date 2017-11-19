using CollegeUni.Controllers;
using CollegeUni.Models;
using CollegeUni.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Xunit;

namespace CollegeUni.Api.Test
{
    public class WhenUserAttemptsLogin
    {
        [Fact]
        public async void ShouldReturnBadRequestObjectResultOnInvalidModelState()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var controller = new AccountController(mockAuthService.Object, mockLogger.Object);
            controller.ModelState.AddModelError("Password", "Required");
            var model = new LoginViewModel
            {
                Email = "marilyn443@example.com",
                Password = string.Empty
            };
            var result = await controller.Token(model);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async void ShouldReturnBadRequestObjectResultOnInvalidCredentials()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var controller = new AccountController(mockAuthService.Object, mockLogger.Object);
            mockAuthService.Setup(svc => svc.GetJwtSecurityToken(It.IsAny<AuthServiceResult>())).Returns(Task.FromResult<JwtSecurityToken>(null));
            var model = new LoginViewModel
            {
                Email = "marilyn443@example.com",
                Password = "EchoLimaBudgie10320"
            };

            var result = await controller.Token(model);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async void ShouldReturnOkResultWithTokenOnValidCredentials()
        {
            var mockAuthService = new Mock<IAuthService>();
            var mockLogger = new Mock<ILogger<AccountController>>();
            var controller = new AccountController(mockAuthService.Object, mockLogger.Object);
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Im1hcmlseW40NDNAZXhhbXBsZS5jb20iLCJhZG1pbiI6dHJ1ZX0.";
            var tokenWithSignature = $"{expectedToken}HNQL2R8fer1A-88xrcW4VPvprtADDjznCvWCTkf7Q10";
            var jwtToken = new JwtSecurityToken(tokenWithSignature);
            mockAuthService.Setup(svc => svc.GetJwtSecurityToken(It.IsAny<AuthServiceResult>())).Returns(Task.FromResult<JwtSecurityToken>(jwtToken));
            var model = new LoginViewModel
            {
                Email = "marilyn443@example.com",
                Password = "EchoLimaBudgie10320"
            };

            var result = await controller.Token(model);
            Assert.IsType<OkObjectResult>(result);
            var response = result as OkObjectResult;
            var actual = response.Value as TokenResponseViewModel;
            Assert.Equal(expectedToken, actual.token);
        }
    }
}
