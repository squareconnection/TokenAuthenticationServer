using System;
using System.Linq;
using TokenAuth.Contracts;
using TokenAuth.Models;
using TokenAuth.Models.ViewModels;
using Xunit;

namespace TokenAuth.Core.Tests
{
    public class UserManagerTests
    {
		IAsyncRepository<IdentityUser> repo;
		UserManager userManager;

        public UserManagerTests()
        {
            repo = new Mocks.MockRepository<IdentityUser>();
            userManager = new UserManager(repo);
        }

		private IdentityRegistrationViewModel GetSingle()
		{
			IdentityRegistrationViewModel model = new IdentityRegistrationViewModel();
			model.Email = "brett.hargreaves@capita.co.uk";
			model.EmployeeId = "12345678";
			model.ProfitCenterId = "ZITX";
			model.FirstName = "Brett";
			model.Surname = "Hargreaves";
			model.Password = "Password123";
			model.ConfirmPassword = "Password123";

			return model;
		}

		private async void LoadMany()
		{
			IdentityRegistrationViewModel brett = new IdentityRegistrationViewModel();
			brett.Email = "brett.hargreaves@capita.co.uk";
			brett.EmployeeId = "12345678";
			brett.ProfitCenterId = "ZITX";
			brett.FirstName = "Brett";
			brett.Surname = "Hargreaves";
			brett.Password = "Password123";
			brett.ConfirmPassword = "Password123";

			IdentityRegistrationViewModel tony = new IdentityRegistrationViewModel();
			tony.Email = "tony.edwards@capita.co.uk";
			tony.EmployeeId = "87654321";
			tony.ProfitCenterId = "ZITX";
			tony.FirstName = "Tony";
			tony.Surname = "Edwards";
			tony.Password = "Password123";
			tony.ConfirmPassword = "Password123";

			IdentityRegistrationViewModel carl = new IdentityRegistrationViewModel();
			carl.Email = "carl.brindle@capita.co.uk";
			carl.EmployeeId = "11112222";
			carl.ProfitCenterId = "ZITX";
			carl.FirstName = "Carl";
			carl.Surname = "Brindle";
			carl.Password = "Password123";
			carl.ConfirmPassword = "Password123";

			await userManager.RegisterAsync(brett);
            await userManager.RegisterAsync(tony);
            await userManager.RegisterAsync(carl);

		}

        [Fact]
        public void UserManager_UserCanRegister()
        {
			IdentityRegistrationViewModel model = GetSingle();

			var result = userManager.Register(model);

			Assert.NotNull(result);
			Assert.True(result.Succeeded);
			Assert.True(result.Errors.Count() == 0);
        }

		[Fact]
		public void UserManager_CanMarkAccountAsConfirmed()
		{
			IdentityRegistrationViewModel model = GetSingle();

			var insertresult = userManager.Register(model);
			Assert.NotNull(insertresult);

			var user = userManager.FindByEmailAsync(model.Email).Result;
            var result = userManager.ConfirmAccount(user.Email, user.SecurityStamp, user.PasswordHash);

			Assert.NotNull(result);
			Assert.True(result.Succeeded);
			Assert.True(result.Errors.Count() == 0);
		}



		[Fact]
		public void UserManager_UserCanSignIn()
		{
			IdentityRegistrationViewModel model = GetSingle();

			var insertresult = userManager.Register(model);
			Assert.NotNull(insertresult);

			userManager.FindByEmailAsync(model.Email);

			var passwordResult = userManager.SignInIdentity(model.Email, model.Password);
			Assert.NotNull(passwordResult);
			Assert.True(passwordResult.Succeeded);
		}

		[Fact]
		public void UserManager_UserSignInWithIncorrectPasswordReturnsError()
		{
			IdentityRegistrationViewModel model = GetSingle();

			var insertresult = userManager.Register(model);
			Assert.NotNull(insertresult);

			var user = userManager.FindByEmailAsync(model.Email);

			var passwordResult = userManager.SignInIdentity(model.Email, "Wrong Password");
			Assert.NotNull(passwordResult);
			Assert.False(passwordResult.Succeeded);
            Assert.True(passwordResult.Errors.Contains("Password Invalid!"));
		}

		[Fact]
		public void UserManager_UserSignInWithNonRegisteredAccountReturnsError()
		{
			IdentityRegistrationViewModel model = GetSingle();

			var insertresult = userManager.Register(model);
			Assert.NotNull(insertresult);

			var user = userManager.FindByEmailAsync(model.Email);

			var passwordResult = userManager.SignInIdentity(model.Email, "Wrong Password");
			Assert.NotNull(passwordResult);
			Assert.False(passwordResult.Succeeded);
			Assert.True(passwordResult.Errors.Contains("User with email address brett.hargreaves@capita.co.uk not found!"));
		}

        [Fact]
        public void UserManager_UserSignInWithNonConfirmedAccountReturnsError()
		{
			IdentityRegistrationViewModel model = GetSingle();

			var insertresult = userManager.Register(model);
			Assert.NotNull(insertresult);

			var user = userManager.FindByEmailAsync(model.Email);

			var passwordResult = userManager.SignInIdentity(model.Email, "Wrong Password");
			Assert.NotNull(passwordResult);
			Assert.False(passwordResult.Succeeded);
			Assert.True(passwordResult.Errors.Contains("Sorry, this account has not been confirmed!  Please check you email for the Activation Link"));
		}

		[Fact]
		public void UserManager_AttemptToRegisterWithNullProfitCenterIdFails()
		{
			IdentityRegistrationViewModel model = GetSingle();
			model.ProfitCenterId = "";


			var result = userManager.Register(model);

			Assert.NotNull(result);
			Assert.False(result.Succeeded);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(result.Errors.FirstOrDefault(), "ProfitCenterId not specified");
		}

		[Fact]
		public void UserManager_AttemptToRegisterWithNullEmployeeIdFails()
		{
			IdentityRegistrationViewModel model = GetSingle();
			model.EmployeeId = "";

			var result = userManager.Register(model);

			Assert.NotNull(result);
			Assert.False(result.Succeeded);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(result.Errors.FirstOrDefault(), "EmployeeId not specified");
		}

		[Fact]
		public void UserManager_AttemptToRegisterWithNullFirstNameFails()
		{
			IdentityRegistrationViewModel model = GetSingle();
			model.FirstName = "";

			var result = userManager.Register(model);

			Assert.NotNull(result);
			Assert.False(result.Succeeded);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(result.Errors.FirstOrDefault(), "First Name not specified");
		}

		[Fact]
		public void UserManager_AttemptToRegisterWithNullSurnameFails()
		{
			IdentityRegistrationViewModel model = GetSingle();
			model.Surname = "";


			var result = userManager.Register(model);

			Assert.NotNull(result);
			Assert.False(result.Succeeded);
			Assert.True(result.Errors.Count() == 1);
			Assert.Equal(result.Errors.FirstOrDefault(), "Surname not specified");
		}

		[Fact]
		public void UserManager_CanSetSecurityToken()
		{
			IdentityRegistrationViewModel model = GetSingle();
			string securityToken = "";

		securityToken = userManager.GenerateSecurityToken();

			Assert.NotNull(securityToken);
			Assert.NotEqual(securityToken, "");
		}

		[Fact]
		public void UserManager_CanAddClaim()
		{
			IdentityRegistrationViewModel model = GetSingle();
			userManager.Register(model);
			var user = userManager.FindByEmail(model.Email);

			if (user != null)
			{
				IdentityUserClaim claim = new IdentityUserClaim()
				{
					UserId = user.Id,
					ClaimType = "TestType",
					ClaimValue = "TestValue"
				};

				userManager.AddClaim(user.Id, claim);

				user = userManager.FindByEmail(model.Email);
			}

			Assert.NotNull(user);
			Assert.True(user.Claims.Count > 0);
			Assert.Equal(user.Claims.FirstOrDefault().ClaimType, "TestType");
			Assert.Equal(user.Claims.FirstOrDefault().ClaimValue, "TestValue");
		}

		[Fact]
		public void UserManager_CanRemoveClaim()
		{
			IdentityRegistrationViewModel model = GetSingle();
			userManager.Register(model);
			var user = userManager.FindByEmail(model.Email);

			if (user != null)
			{
				IdentityUserClaim claim = new IdentityUserClaim()
				{
					UserId = user.Id,
					ClaimType = "TestType",
					ClaimValue = "TestValue"
				};

				IdentityUserClaim claim2 = new IdentityUserClaim()
				{
					UserId = user.Id,
					ClaimType = "Test2",
					ClaimValue = "Test2"
				};

				userManager.AddClaim(user.Id, claim);
				userManager.AddClaim(user.Id, claim2);

				userManager.DeleteClaim(user.Id, claim);

				user = userManager.FindByEmail(model.Email);
			}

			Assert.NotNull(user);
			Assert.True(user.Claims.Count == 1);
			Assert.Equal(user.Claims.FirstOrDefault().ClaimType, "Test2");
			Assert.Equal(user.Claims.FirstOrDefault().ClaimValue, "Test2");
		}
    }
}
