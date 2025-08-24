namespace TeamTaskManagement.Application.DTOs
{
	using System.ComponentModel.DataAnnotations;

	public class InviteUserToTeamDTO
	{
		[Required(ErrorMessage = "Email is required.")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|hotmail\.com|system\.com)$",
			ErrorMessage = "Email must be from gmail.com, yahoo.com, outlook.com, hotmail.com, or system.com")]
		public string Email { get; set; } = default!;
	}
}