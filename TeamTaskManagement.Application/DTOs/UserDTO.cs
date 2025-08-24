namespace TeamTaskManagement.Application.DTOs
{
	public class UserDTO
	{
		public Guid UserId { get; set; }
		public string Email { get; set; } = default!;
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
		public string Role { get; set; } = default!;
		public string TeamRole { get; set; } = default!;
	}
}