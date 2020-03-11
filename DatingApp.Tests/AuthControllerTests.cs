using DatingApp.API.Controllers;
using DatingApp.API.Data;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using DatingApp.API.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Tests
{
	public class AuthControllerTests
	{
		[Fact]
		public void Login_Should_Return_OkObjectResult_With_Token_When_Correct_Login_Data()
		{
			//arrange
			string userName = "Bob";
			string password = "pass123.";
			var controller = new AuthController(_authRepositoryMock.Object, _configurationMock.Object);
			_authRepositoryMock.Setup(x => x.Login(userName.ToLower(), password)).Returns(Task.FromResult(new User { Id = 1, UserName = userName }));

			//act
			var result = controller.Login(new API.Dtos.UserForLoginDto
			{
				Password = password,
				UserName = userName
			}).Result as OkObjectResult;

			//assert
			Assert.IsType<OkObjectResult>(result);
			Assert.NotNull(result.Value);
		}

		[Fact]
		public void Login_Should_Return_UnauthorizedResult_When_Incorrect_Login_Data()
		{
			//arrange
			string userName = "Bob";
			string password = "Some kind of incorrect password for Bob";
			var controller = new AuthController(_authRepositoryMock.Object, _configurationMock.Object);
			_authRepositoryMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

			//act
			var result = controller.Login(new API.Dtos.UserForLoginDto
			{
				Password = password,
				UserName = userName
			}).Result;

			//assert
			Assert.IsType<UnauthorizedResult>(result);
		}

		[Fact]
		public void Register_Should_Return_BadRequest_When_User_Exists()
		{
			//arrange
			string userName = "Bob";

			var controller = new AuthController(_authRepositoryMock.Object, _configurationMock.Object);
			_authRepositoryMock.Setup(x => x.UserExists(userName.ToLower())).Returns(() => Task.FromResult(true));

			//act
			var result = controller.Register(new API.Dtos.UserForRegisterDto
			{
				UserName = "Bob",
				Password = "Example"
			}).Result;

			//assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public void Register_Should_Return_201StatusCode_When_User_Created()
		{
			//arrange
			string userName = "Bob";
						
			var controller = new AuthController(_authRepositoryMock.Object, _configurationMock.Object);
			_authRepositoryMock.Setup(x => x.UserExists(userName.ToLower())).Returns(() => Task.FromResult(false));

			//act
			var result = controller.Register(new API.Dtos.UserForRegisterDto
			{
				UserName = userName,
				Password = "Example"
			}).Result as StatusCodeResult;

			//arrange
			Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(new StatusCodeResult(201).StatusCode, result.StatusCode);
		}

		public AuthControllerTests()
		{
			_authRepositoryMock = new Mock<IAuthRepository>();
			_configurationMock = new Mock<IConfiguration>();

			_configurationMock.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("Some kind of super secret key");
		}

		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IConfiguration> _configurationMock;
	}
}
