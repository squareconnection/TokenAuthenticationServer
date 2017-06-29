using System;
namespace TokenAuth.Models.ViewModels
{
    public class IdentityRegistrationViewModel
    {
        public IdentityRegistrationViewModel()
        {
        }

		public string EmployeeId { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string ProfitCenterId { get; set; }
		public string Email { get; set; }
		public virtual string Telephone { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
    }
}
