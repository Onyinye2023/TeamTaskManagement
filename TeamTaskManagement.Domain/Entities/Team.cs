namespace TeamTaskManagement.Domain.Entities
{
	public class Team
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string Description { get; set; }
		public Guid CreatedByUserId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public User CreatedBy { get; set; } = default!;
		public ICollection<TeamUser> Members { get; set; } = new List<TeamUser>();
		public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
	}
}
