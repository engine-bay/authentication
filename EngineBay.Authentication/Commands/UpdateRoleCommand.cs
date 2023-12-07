namespace EngineBay.Authentication
{
    public class UpdateRoleCommand
    {
        public UpdateRoleCommand(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Guid>? GroupIds { get; set; }

        public ICollection<string>? GroupNames { get; set; }
    }
}