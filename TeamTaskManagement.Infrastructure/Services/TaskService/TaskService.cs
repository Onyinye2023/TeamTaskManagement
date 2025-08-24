namespace TeamTaskManagement.Infrastructure.Services.TaskService
{
	using AutoMapper;
	using Microsoft.Extensions.Logging;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IRepositories.ITaskRepo;
	using TeamTaskManagement.Application.IServices.ITaskService;

	public class TaskService : ITaskService
	{
		private readonly ITaskRepo _taskRepo;
		private readonly IMapper _mapper;
		private readonly ILogger<TaskService> _logger;

		public TaskService(ITaskRepo taskRepo, IMapper mapper, ILogger<TaskService> logger)
		{
			_taskRepo = taskRepo;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTaskDTO, Guid teamId, string creatorId)
		{
			if (!Guid.TryParse(creatorId, out var parsedCreatorId))
			{
				_logger.LogWarning("Invalid CreatorId format: {CreatorId}", creatorId);
				throw new ArgumentException("Invalid CreatorId format.", nameof(creatorId));
			}

			try
			{
				var task = await _taskRepo.CreateTaskAsync(createTaskDTO, teamId, parsedCreatorId);
				return _mapper.Map<TaskDTO>(task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating task for team {TeamId} by creator {CreatorId}", teamId, creatorId);
				throw;
			}
		}

		public async Task<TaskDTO?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO updateTaskDTO, string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				throw new ArgumentException("Invalid UserId format.", nameof(userId));
			}

			try
			{
				var task = await _taskRepo.UpdateTaskAsync(taskId, updateTaskDTO, parsedUserId);
				if (task == null)
				{
					_logger.LogWarning("Task {TaskId} not found or user {UserId} not authorized", taskId, userId);
					return null;
				}
				return _mapper.Map<TaskDTO>(task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating task {TaskId} by user {UserId}", taskId, userId);
				throw;
			}
		}

		public async Task<TaskDTO?> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDTO updateTaskStatusDTO, string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				throw new ArgumentException("Invalid UserId format.", nameof(userId));
			}

			try
			{
				var task = await _taskRepo.UpdateTaskStatusAsync(taskId, updateTaskStatusDTO, parsedUserId);
				if (task == null)
				{
					_logger.LogWarning("Task {TaskId} not found or user {UserId} not authorized", taskId, userId);
					return null;
				}
				return _mapper.Map<TaskDTO>(task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating status of task {TaskId} by user {UserId}", taskId, userId);
				throw;
			}
		}

		public async Task<bool> DeleteTaskAsync(Guid taskId, string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				throw new ArgumentException("Invalid UserId format.", nameof(userId));
			}

			try
			{
				var result = await _taskRepo.DeleteTaskAsync(taskId, parsedUserId);
				if (!result)
				{
					_logger.LogWarning("Task {TaskId} not found or user {UserId} not authorized", taskId, userId);
				}
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting task {TaskId} by user {UserId}", taskId, userId);
				throw;
			}
		}

		public async Task<List<TaskDTO>> GetTasksByTeamAsync(Guid teamId, string userId)
		{
			if (!Guid.TryParse(userId, out var parsedUserId))
			{
				_logger.LogWarning("Invalid UserId format: {UserId}", userId);
				throw new ArgumentException("Invalid UserId format.", nameof(userId));
			}

			try
			{
				var tasks = await _taskRepo.GetTasksByTeamAsync(teamId, parsedUserId);
				return _mapper.Map<List<TaskDTO>>(tasks);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving tasks for team {TeamId} by user {UserId}", teamId, userId);
				throw;
			}
		}
	}
}