namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class RoleDto
    {
        public RoleDto(Role role)
        {
            ArgumentNullException.ThrowIfNull(role);

            this.Id = role.Id;
            this.Name = role.Name;
            this.Description = role.Description;

            if (role.Groups != null)
            {
                this.Groups = role.Groups
                    .Select(x => new GroupDto(x))
                    .ToList();
            }
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<GroupDto>? Groups { get; set; }
    }
}