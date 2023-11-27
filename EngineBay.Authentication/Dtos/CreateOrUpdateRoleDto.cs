namespace EngineBay.Authentication
{
    public class CreateOrUpdateRoleDto
    {
        public CreateOrUpdateRoleDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Guid>? GroupIds { get; set; }
    }
}