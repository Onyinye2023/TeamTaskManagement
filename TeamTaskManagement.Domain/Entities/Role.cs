namespace TeamTaskManagement.Domain.Entities
{
	public class Role
	{
		public int RoleId { get; set; }
		public string? RoleName { get; set; }
		public ICollection<User> Users { get; set; } = new List<User>();
	}
}
