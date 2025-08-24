namespace TeamTaskManagement.Domain.Entities
{
	public class User
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
		public string PasswordHash { get; set; } = default!;
		public string Email { get; set; } = default!;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public int RoleId { get; set; }
		public Role Role { get; set; } 
		public ICollection<TeamUser> Teams { get; set; } = new List<TeamUser>();
		public ICollection<ProjectTask> CreatedTasks { get; set; } = new List<ProjectTask>();
		public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();

	}
}
