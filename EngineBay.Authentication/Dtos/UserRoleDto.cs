namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class UserRoleDto
    {
        public UserRoleDto(UserRole userRole)
        {
            ArgumentNullException.ThrowIfNull(userRole);

            this.Id = userRole.Id;
            this.ApplicationUserId = userRole.ApplicationUserId;

            if (userRole.ApplicationUser != null)
            {
                this.ApplicationUserDto = new ApplicationUserDto(userRole.ApplicationUser);
            }

            if (userRole.Roles != null)
            {
                this.Roles = userRole.Roles
                    .Select(x => new RoleDto(x))
                    .ToList();
            }
        }

        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }

        public ApplicationUserDto? ApplicationUserDto { get; set; }

        public ICollection<RoleDto>? Roles { get; set; }
    }
}