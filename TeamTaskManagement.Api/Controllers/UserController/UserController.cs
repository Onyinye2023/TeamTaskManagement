namespace TeamTaskManagement.Api.Controllers.UserController
{
	using System.Net;
	using System.Security.Claims;
	using AutoMapper;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using TeamTaskManagement.Application.DTOs;
	using TeamTaskManagement.Application.IServices.IUserService;

	/// <summary>
	/// Handles all user-related operations such as registration, login, and profile management.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ILogger<UserController> _logger;
		private readonly IMapper _mapper;

		public UserController(ILogger<UserController> logger, IUserService userService, IMapper mapper)
		{
			_userService = userService;
			_logger = logger;
			_mapper = mapper;
		}

		/// <summary>
		/// Registers a new admin user.
		/// </summary>
		/// <param name="adminDto">The admin registration details.</param>
		/// <param name="roleId">The role ID to assign to the admin.</param>
		/// <returns>A success or failure response with details.</returns>	
		[HttpPost("register-admin")]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO adminDto, [FromQuery] int roleId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var response = await _userService.RegisterAdmin(adminDto, roleId);

			if (!response.Success)
				return BadRequest(response);

			return Ok(response);
		}

		/// <summary>
		/// Registers a normal user (SuperAdmin only).
		/// </summary>
		/// <param name="userDto">The user registration details.</param>
		/// <param name="roleId">The role ID to assign to the user.</param>
		/// <returns>A success or failure response with details.</returns>
		[Authorize(Roles = "SuperAdmin")]
		[HttpPost("auth/create-user")]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> RegisterUser([FromBody] RegisterDTO userDto, [FromQuery] int roleId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var response = await _userService.RegisterUser(userDto, roleId);

			if (!response.Success)
				return BadRequest(response);

			return Ok(response);
		}

		/// <summary>
		/// Authenticates a user and returns a JWT token if successful.
		/// </summary>
		/// <param name="loginDTO">The login details (username and password).</param>
		/// <returns>A JWT token and success message if authentication passes.</returns>
		[HttpPost("auth/login")]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(ResponseDTO), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
		{
			var result = await _userService.UserLogin(loginDTO);
			if (result.Success)
			{
				return Ok(new { result.Token, result.Message });
			}
			return BadRequest(new { result.Message });
		}

		/// <summary>
		/// Retrieves the currently logged-in user's profile information.
		/// </summary>
		/// <returns>User details or an error if not found.</returns>
		[HttpGet("me")]
		[Authorize]
		[ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> GetCurrentUser()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("No UserId found in JWT token.");
				return Unauthorized("User ID not found in token.");
			}

			var user = await _userService.GetCurrentUserAsync(userId);
			if (user == null)
			{
				_logger.LogWarning("User not found for ID: {UserId}", userId);
				return NotFound("User not found.");
			}

			return Ok(user);
		}
	}
}
