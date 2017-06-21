using System;
namespace TokenAuth.Models
{
	public class IdentityUserClaim : EntityBase
	{
		public IdentityUserClaim()
		{
			this.Id = Guid.NewGuid().ToString();
		}
		/// <summary>
		/// Gets or sets the of the primary key of the user associated with this claim.
		/// </summary>
		public virtual string UserId { get; set; }

		/// <summary>
		/// ApplicationId that this claim or role pertains to (if any)
		/// </summary>
		public virtual string ApplicationId { get; set; }
		/// <summary>
		/// Gets or sets the claim type for this claim.
		/// </summary>
		public virtual string ClaimType { get; set; }

		/// <summary>
		/// Gets or sets the claim value for this claim.
		/// </summary>
		public virtual string ClaimValue { get; set; }
	}
}
