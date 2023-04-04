using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Employee.Models.Account
{


	public class DMMRegisterViewModel
	{
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

        [Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "Password and confirmation password not match.")]
		public string ConfirmPassword { get; set; } = null!;

		public int DM { get; set; }

	}
}
