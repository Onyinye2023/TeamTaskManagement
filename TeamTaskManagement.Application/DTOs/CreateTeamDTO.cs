namespace TeamTaskManagement.Application.DTOs
{
	using System.ComponentModel.DataAnnotations;
	public class CreateTeamDTO
	{
		[Required(ErrorMessage = "Team name is required.")]
		[StringLength(100, ErrorMessage = "Team name cannot exceed 100 characters.")]
		public string Name { get; set; } = default!;
		public string Description { get; set; } 

	}
}