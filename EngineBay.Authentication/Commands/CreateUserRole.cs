namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class CreateUserRole : ICommandHandler<CreateUserRoleDto, UserRoleDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreateUserRoleDto> validator;

        public CreateUserRole(
            AuthenticationDbContext authDb,
            IValidator<CreateUserRoleDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<UserRoleDto> Handle(CreateUserRoleDto createUserRoleDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createUserRoleDto);

            await this.validator.ValidateAndThrowAsync(createUserRoleDto, cancellation);

            List<Role>? roles = null;
            if (createUserRoleDto.RoleIds != null || createUserRoleDto.RoleNames != null)
            {
                roles = await this.authDb.Roles.Where(r => (createUserRoleDto.RoleIds != null && createUserRoleDto.RoleIds.Contains(r.Id)) || (createUserRoleDto.RoleNames != null && createUserRoleDto.RoleNames.Contains(r.Name)))
                    .ToListAsync(cancellation);
            }

            var userRole = new UserRole()
            {
                ApplicationUserId = createUserRoleDto.UserId,
                Roles = roles,
            };

            var addedUser = await this.authDb.UserRoles.AddAsync(userRole, cancellation) ??
                            throw new PersistenceException("Did not succesfully add auth user.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new UserRoleDto(addedUser.Entity);
        }
    }
}
