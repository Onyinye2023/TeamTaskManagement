namespace TeamTaskManagement.Application.IRepositories.ITaskRepo
{
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Domain.Entities;

	public interface ITaskRepo
	{
		Task<ProjectTask> CreateTaskAsync(CreateTaskDTO createTaskDTO, Guid teamId, Guid creatorId);
		Task<ProjectTask?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO updateTaskDTO, Guid userId);
		Task<ProjectTask?> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDTO updateTaskStatusDTO, Guid userId);
		Task<bool> DeleteTaskAsync(Guid taskId, Guid userId);
		Task<List<ProjectTask>> GetTasksByTeamAsync(Guid teamId, Guid userId);
	}
}
