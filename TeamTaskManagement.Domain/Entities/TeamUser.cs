namespace TeamTaskManagement.Domain.Entities
{
	using TeamTaskManagement.Domain.Enums;

	public class TeamUser
	{
		public Guid TeamId { get; set; }
		public Guid UserId { get; set; }
		public TeamRole Role { get; set; } = TeamRole.Member;
		public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
		public Team Team { get; set; } = default!;
		public User User { get; set; } = default!;
	}
}
