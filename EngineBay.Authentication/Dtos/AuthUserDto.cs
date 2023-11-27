namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class AuthUserDto
    {
        public AuthUserDto(AuthUser authUser)
        {
            ArgumentNullException.ThrowIfNull(authUser);

            this.Id = authUser.Id;
            this.ApplicationUserId = authUser.ApplicationUserId;

            if (authUser.ApplicationUser != null)
            {
                this.ApplicationUserDto = new ApplicationUserDto(authUser.ApplicationUser);
            }

            if (authUser.Roles != null)
            {
                this.Roles = authUser.Roles
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