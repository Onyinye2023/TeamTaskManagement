using System.ComponentModel.DataAnnotations;

public class CreateTaskDTO
{
	[Required(ErrorMessage = "Task title is required.")]
	[StringLength(100, ErrorMessage = "Task title cannot exceed 100 characters.")]
	public string Title { get; set; } = default!;

	[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
	public string? Description { get; set; }
	public DateTime DueDate { get; set; }
	public Guid? AssignedToUserId { get; set; }
}
