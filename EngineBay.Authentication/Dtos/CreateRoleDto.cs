namespace EngineBay.Authentication
{
    public class CreateRoleDto
    {
        public CreateRoleDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<GroupDto>? Groups { get; set; }
    }
}