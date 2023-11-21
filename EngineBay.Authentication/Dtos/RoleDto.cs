namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class RoleDto
    {
        public RoleDto(Role role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            this.Id = role.Id;
            this.Name = role.Name;
            this.Description = role.Description;

            if (role.Users != null)
            {
                this.AuthUserDtos = role.Users
                    .Select(x => new AuthUserDto(x))
                    .ToList();
            }
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<AuthUserDto>? AuthUserDtos { get; set; }
    }
}