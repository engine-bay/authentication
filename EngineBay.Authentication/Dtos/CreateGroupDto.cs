namespace EngineBay.Authentication
{
    public class CreateGroupDto
    {
        public CreateGroupDto(string name)
        {
            this.Name = name;
        }

        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Guid>? PermissionIds { get; set; }

        public ICollection<string>? PermissionNames { get; set; }
    }
}