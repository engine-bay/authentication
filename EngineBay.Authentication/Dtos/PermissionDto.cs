namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class PermissionDto
    {
        public PermissionDto(Permission permission)
        {
            ArgumentNullException.ThrowIfNull(permission);

            this.Id = permission.Id;
            this.Name = permission.Name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}