namespace TeamTaskManagement.Application.DTOs
{
	public class TeamDTO
	{
		public Guid TeamId { get; set; }
		public string Name { get; set; } = default!;
		public string Description { get; set; } = default!;
		public Guid CreatedById { get; set; }
		public string CreatedByEmail { get; set; } = default!;
		public DateTime CreatedAt { get; set; }
	}
}