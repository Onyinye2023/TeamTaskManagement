namespace TeamTaskManagement.Application.IServices.ITaskService
{
	using TeamTaskManagement.Application.DTOs;

	public interface ITaskService
	{
		Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTaskDTO, Guid teamId, string creatorId);
		Task<TaskDTO?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO updateTaskDTO, string userId);
		Task<TaskDTO?> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDTO updateTaskStatusDTO, string userId);
		Task<bool> DeleteTaskAsync(Guid taskId, string userId);
		Task<List<TaskDTO>> GetTasksByTeamAsync(Guid teamId, string userId);
	}
}
