namespace TeamTaskManagement.Domain.Entities
{
	using TeamTaskManagement.Domain.Enums;

	public class ProjectTask
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = default!;
		public string? Description { get; set; }
		public DateTime? DueDate { get; set; }
		public TasksStatus Status { get; set; } = TasksStatus.Pending;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public Guid TeamId { get; set; }
		public Team Team { get; set; } = default!;
		public Guid CreatedByUserId { get; set; }
		public User CreatedBy { get; set; } = default!;
		public Guid? AssignedToUserId { get; set; }
		public User? AssignedTo { get; set; }
		public bool IsDeleted { get; set; }
	}
}
