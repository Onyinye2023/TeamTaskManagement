namespace TeamTaskManagement.Infrastructure.Repositories.TaskRepo
{
	using Microsoft.Extensions.Logging;
	using TeamTaskManagement.Infrastructure.Data;
	using TeamTaskManagement.Application.IRepositories.ITaskRepo;
	using TeamTaskManagement.Domain.Entities;
	using Microsoft.EntityFrameworkCore;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Domain.Enums;

	public class TaskRepo : ITaskRepo
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TaskRepo> _logger;

		public TaskRepo(AppDbContext context, ILogger<TaskRepo> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ProjectTask> CreateTaskAsync(CreateTaskDTO createTaskDTO, Guid teamId, Guid creatorId)
		{
			try
			{
				var team = await _context.Teams.FindAsync(teamId);
				if (team == null)
				{
					_logger.LogWarning("Team not found for ID: {TeamId}", teamId);
					throw new ArgumentException("Team not found.", nameof(teamId));
				}

				var creator = await _context.Users.FindAsync(creatorId);
				if (creator == null)
				{
					_logger.LogWarning("Creator not found for ID: {CreatorId}", creatorId);
					throw new ArgumentException("Creator user not found.", nameof(creatorId));
				}

				User? assignedTo = null;
				if (createTaskDTO.AssignedToUserId.HasValue) 
				{
					assignedTo = await _context.Users.FindAsync(createTaskDTO.AssignedToUserId.Value);
					if (assignedTo == null)
					{
						_logger.LogWarning("Assigned user not found for ID: {AssignedToUserId}", createTaskDTO.AssignedToUserId);
						
					}
				}

				var task = new ProjectTask
				{
					Id = Guid.NewGuid(),
					Title = createTaskDTO.Title,
					Description = createTaskDTO.Description,
					TeamId = teamId,
					CreatedByUserId = creatorId,
					CreatedAt = DateTime.UtcNow,
					AssignedToUserId = assignedTo?.UserId,  
					AssignedTo = assignedTo
				};

				_context.Tasks.Add(task);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Task {TaskId} created for team {TeamId} by user {CreatorId}, assigned to {AssignedToUserId}",
					task.Id, teamId, creatorId, task.AssignedToUserId);

				return task;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating task for team {TeamId} by creator {CreatorId}", teamId, creatorId);
				throw new Exception("An error occurred while creating the task.", ex);
			}
		}

		public async Task<ProjectTask?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO updateTaskDTO, Guid userId)
		{
			try
			{
				var task = await _context.Tasks
					.Include(t => t.Team)
					.FirstOrDefaultAsync(t => t.Id == taskId);

				if (task == null)
				{
					_logger.LogWarning("Task not found for ID: {TaskId}", taskId);
					return null;
				}

				var isTeamMember = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == userId && ut.TeamId == task.TeamId);

				if (!isTeamMember)
				{
					_logger.LogWarning("User {UserId} is not authorized to update task {TaskId} in team {TeamId}",
						userId, taskId, task.TeamId);
					return null;
				}

				task.Title = updateTaskDTO.Title ?? task.Title;
				task.Description = updateTaskDTO.Description ?? task.Description;
				task.DueDate = updateTaskDTO.DueDate ?? task.DueDate;

				if (updateTaskDTO.AssignedToUserId.HasValue)
				{
					var assignedUser = await _context.Users.FindAsync(updateTaskDTO.AssignedToUserId.Value);
					if (assignedUser != null)
					{
						task.AssignedToUserId = assignedUser.UserId;
						task.AssignedTo = assignedUser;
						_logger.LogInformation("Task {TaskId} reassigned to user {AssignedUserId}", taskId, assignedUser.UserId);
					}
					else
					{
						_logger.LogWarning("Assigned user not found for ID: {AssignedToUserId}. Assignment skipped.",
							updateTaskDTO.AssignedToUserId);
						task.AssignedToUserId = null;
						task.AssignedTo = null;
					}
				}

				_context.Tasks.Update(task);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Task {TaskId} updated by user {UserId}", taskId, userId);
				return task;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating task {TaskId} by user {UserId}", taskId, userId);
				throw new Exception("An error occurred while updating the task.", ex);
			}
		}

		public async Task<ProjectTask?> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDTO updateTaskStatusDTO, Guid userId)
		{
			try
			{
				var task = await _context.Tasks
					.Include(t => t.Team)
					.FirstOrDefaultAsync(t => t.Id == taskId);
				if (task == null)
				{
					_logger.LogWarning("Task not found for ID: {TaskId}", taskId);
					return null;
				}

				var isTeamMember = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == userId && ut.TeamId == task.TeamId);

				if (!isTeamMember)
				{
					_logger.LogWarning("User {UserId} is not authorized to update status of task {TaskId} in team {TeamId}", userId, taskId, task.TeamId);
					return null;
				}

				task.Status = updateTaskStatusDTO.Status;

				_context.Tasks.Update(task);
				await _context.SaveChangesAsync();
				_logger.LogInformation("Task {TaskId} status updated to {Status} by user {UserId}", taskId, updateTaskStatusDTO.Status, userId);
				return task;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating status of task {TaskId} by user {UserId}", taskId, userId);
				throw new Exception("An error occurred while updating the task status.", ex);
			}
		}

		public async Task<bool> DeleteTaskAsync(Guid taskId, Guid userId)
		{
			try
			{
				var task = await _context.Tasks
					.FirstOrDefaultAsync(t => t.Id == taskId);
				if (task == null)
				{
					_logger.LogWarning("Task not found for ID: {TaskId}", taskId);
					return false;
				}

				var isCreator = task.CreatedByUserId == userId;
				var isTeamAdmin = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == userId && ut.TeamId == task.TeamId && ut.Role == TeamRole.TeamAdmin);
				var isSuperAdmin = await _context.Users
					.AnyAsync(u => u.UserId == userId && u.Role.RoleName == "SuperAdmin");

				if (!isCreator && !isTeamAdmin && !isSuperAdmin)
				{
					_logger.LogWarning("User {UserId} is not authorized to delete task {TaskId} in team {TeamId}", userId, taskId, task.TeamId);
					return false;
				}

				_context.Tasks.Remove(task);
				await _context.SaveChangesAsync();
				_logger.LogInformation("Task {TaskId} deleted by user {UserId}", taskId, userId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting task {TaskId} by user {UserId}", taskId, userId);
				throw new Exception("An error occurred while deleting the task.", ex);
			}
		}

		public async Task<List<ProjectTask>> GetTasksByTeamAsync(Guid teamId, Guid userId)
		{
			try
			{
				var team = await _context.Teams.FindAsync(teamId);
				if (team == null)
				{
					_logger.LogWarning("Team not found for ID: {TeamId}", teamId);
					throw new ArgumentException("Team not found.", nameof(teamId));
				}

				var isTeamMember = await _context.TeamUsers
					.AnyAsync(ut => ut.UserId == userId && ut.TeamId == teamId);
				var isSuperAdmin = await _context.Users
					.AnyAsync(u => u.UserId == userId && u.Role.RoleName == "SuperAdmin");

				if (!isTeamMember && !isSuperAdmin)
				{
					_logger.LogWarning("User {UserId} is not authorized to view tasks for team {TeamId}", userId, teamId);
					return new List<ProjectTask>();
				}

				var tasks = await _context.Tasks
					.Include(t => t.CreatedBy)
					.Where(t => t.TeamId == teamId)
					.ToListAsync();

				_logger.LogInformation("Retrieved {TaskCount} tasks for team {TeamId} by user {UserId}", tasks.Count, teamId, userId);
				return tasks;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving tasks for team {TeamId} by user {UserId}", teamId, userId);
				throw new Exception("An error occurred while retrieving tasks.", ex);
			}
		}
	}
}