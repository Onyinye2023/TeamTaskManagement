namespace TeamTaskManagement.Api.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using System.Collections.Generic;
	using System.Net;
	using System.Security.Claims;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IServices.ITaskService;

	/// <summary>
	/// Controller for managing tasks within teams.
	/// Provides endpoints for creating, updating, deleting, and retrieving tasks.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class TaskController : ControllerBase
	{
		private readonly ITaskService _taskService;
		private readonly ILogger<TaskController> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskController"/>.
		/// </summary>
		/// <param name="taskService">The service used for task operations.</param>
		/// <param name="logger">The logger instance for logging information.</param>
		public TaskController(ITaskService taskService, ILogger<TaskController> logger)
		{
			_taskService = taskService;
			_logger = logger;
		}

		/// <summary>
		/// Creates a new task for a given team.
		/// </summary>
		/// <param name="teamId">The ID of the team.</param>
		/// <param name="createTaskDTO">The task details to create.</param>
		/// <returns>Returns the created task details with a success message.</returns>
		[Authorize(Roles = "User")]
		[HttpPost("/teams/{teamId}/tasks")]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> CreateTask(Guid teamId, [FromBody] CreateTaskDTO createTaskDTO)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for CreateTaskDTO.");
				return BadRequest(ModelState);
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("No UserId found in JWT token.");
				return Unauthorized("User ID not found in token.");
			}

			try
			{
				var task = await _taskService.CreateTaskAsync(createTaskDTO, teamId, userId);
				return Ok(new
				{
					Message = "You have successfully created a task for this team.",
					task
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating task for team {TeamId} by user {UserId}", teamId, userId);
				return StatusCode(500, "An error occurred while creating the task.");
			}
		}

		/// <summary>
		/// Updates an existing task.
		/// </summary>
		/// <param name="taskId">The ID of the task to update.</param>
		/// <param name="updateTaskDto">The updated task details.</param>
		/// <returns>Returns the updated task with a success message.</returns>
		[HttpPut("/tasks/{taskId}")]
		[Authorize(Roles = "User")]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDTO updateTaskDto)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for UpdateTaskDTO.");
				return BadRequest(ModelState);
			}

			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var updatedTask = await _taskService.UpdateTaskAsync(taskId, updateTaskDto, userId);
				return Ok(new { Message = "Task updated successfully.", Task = updatedTask });
			}
			catch (KeyNotFoundException ex)
			{
				_logger.LogWarning(ex, "Task not found for TaskId: {TaskId}", taskId);
				return NotFound(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized update attempt for TaskId: {TaskId}", taskId);
				return Forbid(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating task with ID: {TaskId}", taskId);
				return StatusCode(500, "An error occurred while updating the task.");
			}
		}

		/// <summary>
		/// Updates the status of a task.
		/// </summary>
		/// <param name="taskId">The ID of the task.</param>
		/// <param name="updateTaskStatusDto">The new task status details.</param>
		/// <returns>Returns the updated task status with a success message.</returns>
		[HttpPatch("/tasks/{taskId}/status")]
		[Authorize(Roles = "User")]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(TaskDTO), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] UpdateTaskStatusDTO updateTaskStatusDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var updatedTask = await _taskService.UpdateTaskStatusAsync(taskId, updateTaskStatusDto, userId);
				return Ok(new { Message = "Task status updated successfully.", Task = updatedTask });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating status for TaskId: {TaskId}", taskId);
				return StatusCode(500, "An error occurred while updating task status.");
			}
		}

		/// <summary>
		/// Deletes a task.
		/// </summary>
		/// <param name="taskId">The ID of the task to delete.</param>
		/// <returns>Returns NoContent if the task was deleted successfully.</returns>
		[HttpDelete("/tasks/{taskId}")]
		[Authorize]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]

		public async Task<IActionResult> DeleteTask(Guid taskId)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				await _taskService.DeleteTaskAsync(taskId, userId);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting TaskId: {TaskId}", taskId);
				return StatusCode(500, "An error occurred while deleting the task.");
			}
		}

		/// <summary>
		/// Retrieves all tasks for a specific team.
		/// </summary>
		/// <param name="teamId">The ID of the team.</param>
		/// <returns>Returns a list of tasks for the specified team.</returns>
		[HttpGet("/team/{teamId}/tasks")]
		[Authorize]
		[ProducesResponseType(typeof(List<TaskDTO>), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(List<TaskDTO>), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> GetTasksByTeam(Guid teamId)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var tasks = await _taskService.GetTasksByTeamAsync(teamId, userId);
				return Ok(tasks);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Forbid(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving tasks for TeamId: {TeamId}", teamId);
				return StatusCode(500, "An error occurred while retrieving tasks.");
			}
		}
	}
}
