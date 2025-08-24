namespace TeamTaskManagement.Application.DTOs
{
	using TeamTaskManagement.Domain.Enums;

	public class TaskDTO
	{
		public Guid TaskId { get; set; }
		public string Title { get; set; } = default!;
		public string? Description { get; set; }
		public TasksStatus Status { get; set; }
		public Guid TeamId { get; set; }
		public Guid CreatedById { get; set; }
		public string CreatedByEmail { get; set; } = default!;
		public DateTime CreatedAt { get; set; }
		public Guid? AssignedToUserId { get; set; }
		public DateTime? DueDate { get; set; }
	}
}