using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Threading;
using System.Threading.Tasks;
using TokenAuth.Contracts;
using TokenAuth.Models;
using TokenAuth.Models.ViewModels;
using System.Runtime.CompilerServices;

namespace TokenAuth.Core
{
    public class UserManager
    {
        IAsyncRepository<IdentityUser> userRepository;

        public UserManager(IAsyncRepository<IdentityUser> userRepository){
            this.userRepository = userRepository;
        }

		/// <summary>
		/// Gets the user, if any, associated with the specified, normalized email address.
		/// </summary>
		/// <param name="Email">The normalized email address to return the user for.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
		/// </returns>
		public virtual Task<IdentityUser> FindByEmailAsync(string Email, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return userRepository.FirstOrDefaultAsync(u => u.Email == Email);
		}

		/// <summary>
		/// Gets the user, if any, associated with the specified, normalized email address.
		/// </summary>
		/// <param name="Email">The normalized email address to return the user for.</param>
		/// <returns>
		/// The user if any associated with the specified normalized email address.
		/// </returns>
		public virtual IdentityUser FindByEmail(string Email)
		{
			return userRepository.FirstOrDefaultAsync(u => u.Email == Email).Result;
		}

        public virtual IdentityResult Register(IdentityRegistrationViewModel model){
            return RegisterAsync(model).Result;
        }
	
		/// <summary>
		/// Registers a user asynchronously
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<IdentityResult> RegisterAsync(IdentityRegistrationViewModel model)
		{
			IdentityResult result = ValidateUser(model);

			if (result.Succeeded)
			{
				IdentityUser user = new IdentityUser();
				user.Email = model.Email;
				user.FirstName = model.FirstName;
				user.Surname = model.Surname;

				user.TwoFactorEnabled = false;
				user.AccessFailedCount = 0;
				user.EmailConfirmed = false;
				user.LastPasswordChange = DateTime.Now;

				user.PasswordHash = GeneratePasswordHash(model.Password);
                user.SecurityStamp = GenerateSecurityToken();


				userRepository.Insert(user);

                //now add the claims such as EmployeeId, ProfitCenter etc.

				try
				{
					await userRepository.CommitAsync();
					//logger.Debug("Successfully registered user " + user.Email);
					result.UserId = user.Id;
					result.SecurityStamp = user.SecurityStamp;
				}
				catch (Exception ex)
				{
					result = IdentityResult.Failed(new List<string>() { ex.Message });
					//logger.Fatal("Error Registering User " + new List<string>() { ex.Message });
				}
			}

			return result;
		}

        public IdentityResult ConfirmAccount(string email, string securityStamp, string passwordHash){
            return ConfirmAccountAsync(email, securityStamp, passwordHash).Result;
        }

        public async Task<IdentityResult> ConfirmAccountAsync(string email, string securityStamp, string passwordHash)
		{
			List<string> errors = new List<string>();

			var user = FindByEmail(email);
			if (user != null)
			{
				if (user.SecurityStamp == securityStamp && VerifyHashedPassword(passwordHash, user.PasswordHash))
				{
					try
					{
						user.EmailConfirmed = true;
                        user.SecurityStamp = await GenerateSecurityTokenAsync();
						await userRepository.CommitAsync();
						return IdentityResult.Success;
					}
					catch (Exception ex)
					{
						errors.Add(ex.Message);
					}
				}
				else
				{
					errors.Add("Token or password was incorrect");
				}
			}
			else
			{
				errors.Add("User not found");
			}

			return IdentityResult.Failed(errors);
		}

        public IdentityResult SignInIdentity(string email, string password)
        {
            return SignInIdentityAsync(email, password).Result;
        }

		public async Task<IdentityResult> SignInIdentityAsync(string email, string password)
		{
            List<string> errors = new List<string>();

            var user = await FindByEmailAsync(email);

            //add a list of userloginvalidators here

			if (user != null)
			{
				if (user.EmailConfirmed)
				{
					if (!VerifyHashedPassword(password, user.PasswordHash))
					{
						//logger.Error("User " + user.Email + " login failed.  sent password was " + Password + " and the stored hash is " + user.PasswordHash);

						errors.Add("Password Invalid!");
					}
				}
				else
				{
					errors.Add("Sorry, this account has not been confirmed!  Please check you email for the Activation Link");
				}
			}
			else
			{
				errors.Add("User with email address " + email + " not found!");
			}

			if (errors.Count > 0)
				return IdentityResult.Failed(errors);
			else
			{
				var result = new IdentityResult();
				result.User = user;
				return result;
			}
        }

		public void DeleteClaim(string userId, IdentityUserClaim claim)
		{
			throw new NotImplementedException();
		}

		public void AddClaim(string userId, IdentityUserClaim claim)
		{
			throw new NotImplementedException();
		}


		public IdentityResult ValidateUser(IdentityRegistrationViewModel model)
		{
			List<string> errors = new List<string>();

			if (FindByEmail(model.Email) != null)
			{
				errors.Add("User with email " + model.Email + " already exists!");
			}

			if (!model.Email.ToLower().Contains("@capita.co.uk"))
			{
				errors.Add("Only capita.co.uk emails are allowed!");
			}

			if (model.Password != model.ConfirmPassword)
				errors.Add("Passwords do not match");

			if (string.IsNullOrEmpty(model.ProfitCenterId))
				errors.Add("ProfitCenterId not specified");

			if (string.IsNullOrEmpty(model.Email))
				errors.Add("Email not specified");

			if (string.IsNullOrEmpty(model.EmployeeId))
				errors.Add("EmployeeId not specified");

			if (string.IsNullOrEmpty(model.FirstName))
				errors.Add("First Name not specified");

			if (string.IsNullOrEmpty(model.Surname))
				errors.Add("Surname not specified");

			var passwordErrors = ValidatePassword(model.Password);
			if (passwordErrors.Count > 0)
			{
				foreach (var e in passwordErrors)
				{
					errors.Add(e);
				}
			}

			if (errors.Count > 0)
				return IdentityResult.Failed(errors);
			else
				return IdentityResult.Success;
		}

		public List<string> ValidatePassword(string passord)
		{
			List<string> errors = new List<string>();

			if (passord.Length < 8)
			{
				errors.Add("Passwordis less than 8 charatcers in length!");
			}

			if (!System.Text.RegularExpressions.Regex.IsMatch(passord, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,15}$"))
			{
				errors.Add("Password does not match complexity requirements!");
			}

			if (passord.ToLower() == "capita")
			{
				errors.Add("Password cannot be Capita or any variation of Capita!");
			}

			if (passord.ToLower() == "password")
			{
				errors.Add("Password cannot be Password or any variation of Password!");
			}

			return errors;
		}

		public virtual async Task<string> GenerateSecurityTokenAsync()
		{
			return await Task.Run(()=>GenerateSecurityToken());
		}

		public virtual string GenerateSecurityToken()
		{
			var key = new byte[32];
			RandomNumberGenerator.Create().GetBytes(key);
			var token = Convert.ToBase64String(key);

			return token;
		}

		internal string GeneratePasswordHash(string password)
		{
            return Convert.ToBase64String(HashPassword(password));
		}

        private static byte[] HashPassword(string password){
            int iterCount = 1000;
            int saltSize = 128 / 8;
            byte[] salt = new byte[saltSize];

            RandomNumberGenerator.Create().GetBytes(salt);
            KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;

            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, 32);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;// format marker

            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
			WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
			WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
			Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
			Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
			return outputBytes;
        }

        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword){
			if (hashedPassword == null)
			{
				throw new ArgumentNullException(nameof(hashedPassword));
			}
			if (providedPassword == null)
			{
				throw new ArgumentNullException(nameof(providedPassword));
			}

			byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

			// read the format marker from the hashed password
			if (decodedHashedPassword.Length == 0)
			{
                return false;
			}

            return VerifyHashedPassword(decodedHashedPassword, providedPassword);
		}

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password){
            int iterCount = 1000;
			try
			{
				// Read header information
				KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
				iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
				int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

				// Read the salt: must be >= 128 bits
				if (saltLength < 128 / 8)
				{
					return false;
				}
				byte[] salt = new byte[saltLength];
				Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

				// Read the subkey (the rest of the payload): must be >= 128 bits
				int subkeyLength = hashedPassword.Length - 13 - salt.Length;
				if (subkeyLength < 128 / 8)
				{
					return false;
				}
				byte[] expectedSubkey = new byte[subkeyLength];
				Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

				// Hash the incoming password and verify it
				byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
				return ByteArraysEqual(actualSubkey, expectedSubkey);
			}
			catch
			{
				// This should never occur except in the case of a malformed payload, where
				// we might go off the end of the array. Regardless, a malformed payload
				// implies verification failed.
				return false;
			}


        }

		// Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static bool ByteArraysEqual(byte[] a, byte[] b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			var areSame = true;
			for (var i = 0; i < a.Length; i++)
			{
				areSame &= (a[i] == b[i]);
			}
			return areSame;
		}


        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
		{
			return ((uint)(buffer[offset + 0]) << 24)
				| ((uint)(buffer[offset + 1]) << 16)
				| ((uint)(buffer[offset + 2]) << 8)
				| ((uint)(buffer[offset + 3]));
		}

		private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
		{
			buffer[offset + 0] = (byte)(value >> 24);
			buffer[offset + 1] = (byte)(value >> 16);
			buffer[offset + 2] = (byte)(value >> 8);
			buffer[offset + 3] = (byte)(value >> 0);
		}
    }
}
