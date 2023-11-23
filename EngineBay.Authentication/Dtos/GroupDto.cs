namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class GroupDto
    {
        public GroupDto(Group group)
        {
            ArgumentNullException.ThrowIfNull(group);

            this.Id = group.Id;
            this.Name = group.Name;
            this.Description = group.Description;

            if (group.Permissions != null)
            {
                this.Permissions = group.Permissions
                    .Select(x => new PermissionDto(x))
                    .ToList();
            }
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<PermissionDto>? Permissions { get; set; }
    }
}