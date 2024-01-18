namespace EngineBay.Authentication
{
    public class CreateUserRoleDto
    {
        public CreateUserRoleDto(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; set; }

        public ICollection<Guid>? RoleIds { get; set; }

        public ICollection<string>? RoleNames { get; set; }
    }
}