namespace TeamTaskManagement.Application.DTOs
{
	using System.ComponentModel.DataAnnotations;
	using TeamTaskManagement.Domain.Enums;

	public class UpdateTaskStatusDTO
	{
		[Required(ErrorMessage = "Status is required.")]
		public TasksStatus Status { get; set; }
	}
}