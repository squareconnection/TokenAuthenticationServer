using System;
using System.Collections.Generic;

namespace TokenAuth.Models
{
    public class IdentityUser : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        public IdentityUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        /// <param name="Email">The user Email.</param>
        public IdentityUser(string email) : this()
        {
            Email = email;
        }

        /// <summary>
        /// Get's or sets the user's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's surname
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a contact telephone number for the user 
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a security Stamp.  This security stamp is used to hold generated key's for use in password resets etc.
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if this user is locked out.
        /// </summary>
        /// <value>True if the user is locked out, otherwise false.</value>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        public virtual DateTimeOffset? LastPasswordChange { get; set; }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public ICollection<IdentityUserClaim> Claims { get; } = new List<IdentityUserClaim>();


        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
        {
            return Email;
        }
    }
}
