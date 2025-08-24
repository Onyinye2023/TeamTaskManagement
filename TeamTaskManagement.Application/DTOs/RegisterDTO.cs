namespace TeamTaskManagement.Application.DTOs
{
	using System.ComponentModel.DataAnnotations;
	using Swashbuckle.AspNetCore.Annotations;

	public class RegisterDTO
	{
		[Required]
		public string FirstName { get; set; } = string.Empty;
		[Required]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|system\.com|hotmail\.com)$")]
		[SwaggerSchema(Description = "user@gmail.com")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = " Password is required")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()><|{}]).{8,}$",
			ErrorMessage = "Password must contain at least one capital letter, " +
			"one small letter, one special character: !@#$%^&*()<>?|], and a minimum of eight characters")]
		//[SwaggerSchema(Description = "password123")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Confirm password required")]
		[Compare("Password", ErrorMessage = "Confirm password must match the password")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
