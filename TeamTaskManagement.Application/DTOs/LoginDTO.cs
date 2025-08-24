namespace TeamTaskManagement.Application.DTOs
{
	using System.ComponentModel.DataAnnotations;
	using Swashbuckle.AspNetCore.Annotations;

	public class LoginDTO
	{
		[Required(ErrorMessage = "Email is required.")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|system\.com|hotmail\.com)$",
			ErrorMessage = "Email must be from gmail.com, yahoo.com, outlook.com, or hotmail.com")]
		[SwaggerSchema(Description = "user@gmail.com")]
		public string Email { get; set; } = default!;

		[Required(ErrorMessage = "Password is required.")]
		[SwaggerSchema(Description = "password123")]
		public string Password { get; set; } = default!;
	}
}
