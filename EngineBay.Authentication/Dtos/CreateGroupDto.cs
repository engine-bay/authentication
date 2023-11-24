namespace EngineBay.Authentication
{
    public class CreateGroupDto
    {
        public CreateGroupDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<PermissionDto>? Permissions { get; set; }
    }
}