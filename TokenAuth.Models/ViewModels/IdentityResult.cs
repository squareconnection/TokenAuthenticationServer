﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TokenAuth.Models.ViewModels
{
	public enum IdentityResultType
	{
		LoginRequest,
		RegisterRequest,
		ConfirmAccountRequest,
		PasswordResetRequest
	}

	public class IdentityResult
	{
		private static readonly IdentityResult _success = new IdentityResult { Succeeded = true };
		private List<string> _errors = new List<string>();

		/// <summary>
		/// Flag indicating whether if the operation succeeded or not.
		/// </summary>
		/// <value>True if the operation succeeded, otherwise false.</value>
		public bool Succeeded { get; protected set; }
		public IdentityResultType IdentityResultType { get; set; }

		public IdentityUser User { get; set; }
		public string UserId { get; set; }
		public string SecurityStamp { get; set; }

		/// <summary>
		/// An <see cref="IEnumerable{T}"/> of <see cref="string"/>s containing an errors
		/// that occurred during the identity operation.
		/// </summary>
		/// <value>An <see cref="IEnumerable{T}"/> of <see cref="string"/>s.</value>
		public IEnumerable<string> Errors => _errors;

		/// <summary>
		/// Returns an <see cref="IdentityResult"/> indicating a successful identity operation.
		/// </summary>
		/// <returns>An <see cref="IdentityResult"/> indicating a successful operation.</returns>
		public static IdentityResult Success => _success;

		/// <summary>
		/// Creates an <see cref="IdentityResult"/> indicating a failed identity operation, with a list of <paramref name="errors"/> if applicable.
		/// </summary>
		/// <param name="errors">An optional array of <see cref="string"/>s which caused the operation to fail.</param>
		/// <returns>An <see cref="IdentityResult"/> indicating a failed identity operation, with a list of <paramref name="errors"/> if applicable.</returns>
		public static IdentityResult Failed(List<string> errors)
		{
			var result = new IdentityResult { Succeeded = false };
			if (errors != null)
			{
				result._errors.AddRange(errors);
			}
			return result;
		}

		/// <summary>
		/// Converts the value of the current <see cref="IdentityResult"/> object to its equivalent string representation.
		/// </summary>
		/// <returns>A string representation of the current <see cref="IdentityResult"/> object.</returns>
		/// <remarks>
		/// If the operation was successful the ToString() will return "Succeeded" otherwise it returned 
		/// "Failed : " followed by a comma delimited list of error codes from its <see cref="Errors"/> collection, if any.
		/// </remarks>
		public override string ToString()
		{
			return Succeeded ?
				   "Succeeded" :
				   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x).ToList()));
		}
	}
}
