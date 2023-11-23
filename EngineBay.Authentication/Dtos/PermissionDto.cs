namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class PermissionDto
    {
        public PermissionDto(Permission permission)
        {
            if (permission is null)
            {
                throw new ArgumentNullException(nameof(permission));
            }

            this.Id = permission.Id;
            this.Name = permission.Name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}