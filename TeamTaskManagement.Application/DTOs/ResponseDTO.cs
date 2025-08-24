namespace TeamTaskManagement.Application.DTOs
{
	public class ResponseDTO
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? Token { get; set; }
	}
}
