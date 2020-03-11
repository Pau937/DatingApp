using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace DatingApp.Tests
{
	public class AuthRepositoryTests
	{
		[Theory]
		[InlineData("Jack", "pass123.")]
		[InlineData("Bob", "pass1111")]
		[InlineData("Agnes", "pass321.")]
		public async void Login_Should_Return_User_With_Correct_Login_Data(string userName, string password)
		{
			using (var context = new DataContext(_options))
			{
				//arrange
				var authRepository = new AuthRepository(context);
				//act
				var result = await authRepository.Login(userName, password);
				//assert
				Assert.NotNull(result);
			}
		}

		[Theory]
		[InlineData("Jack", "pass123.")]
		public async void Register_Shoud_Not_To_Be_Able_Register_Two_Users_With_The_Same_Username(string userName, string password)
		{
			_options = new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(databaseName: "Users_Database2")
				.Options;

			using (var context = new DataContext(_options))
			{
				//arrange
				var authRepository = new AuthRepository(context);
				//act
				var firstUser = await authRepository.Register(new User { UserName = userName }, password);
				//assert
				Assert.NotNull(firstUser);
				await Assert.ThrowsAsync<Exception>(() => authRepository.Register(new User { UserName = userName }, password));
			}
		}

		[Theory]
		[InlineData("Jack", "pass12.")]
		[InlineData("Boob", "pass1111")]
		[InlineData("Agns", "pas321.")]
		public async void Login_Should_Return_Null_When_Wrong_Password_Or_UserName(string userName, string password)
		{
			using (var context = new DataContext(_options))
			{
				//arrange
				var authRepository = new AuthRepository(context);
				//act
				var result = await authRepository.Login(userName, password);
				//assert
				Assert.Null(result);
			}
		}

		[Theory]
		[InlineData("Jack")]
		[InlineData("Bob")]
		[InlineData("Agnes")]
		public async void UserExists_Should_Return_True_If_User_Exists(string userName)
		{
			using (var context = new DataContext(_options))
			{
				//arrange
				var authRepository = new AuthRepository(context);
				//act
				var result = await authRepository.UserExists(userName);
				//assert
				Assert.True(result);
			}
		}

		public AuthRepositoryTests()
		{
			_options = new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(databaseName: "Users_Database")
				.Options;

			using (var context = new DataContext(_options))
			{
				if (!context.Users.Any())
				{
					using (var hmac = new System.Security.Cryptography.HMACSHA512())
					{
						context.AddRange(new User
						{
							Id = 1,
							UserName = "Jack",
							PasswordSalt = hmac.Key,
							PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("pass123."))
						},
						new User
						{
							Id = 2,
							UserName = "Bob",
							PasswordSalt = hmac.Key,
							PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("pass1111"))
						},
						new User
						{
							Id = 3,
							UserName = "Agnes",
							PasswordSalt = hmac.Key,
							PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("pass321."))
						});
					}

					context.SaveChanges();
				}
			}
		}

		private DbContextOptions _options;
	}
}
