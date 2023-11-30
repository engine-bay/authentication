namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class CreateRole : ICommandHandler<CreateOrUpdateRoleDto, RoleDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreateOrUpdateRoleDto> validator;

        public CreateRole(
            AuthenticationDbContext authDb,
            IValidator<CreateOrUpdateRoleDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<RoleDto> Handle(CreateOrUpdateRoleDto createRoleDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createRoleDto);

            this.validator.ValidateAndThrow(createRoleDto);

            List<Group>? groups = null;
            if (createRoleDto.GroupIds != null)
            {
                groups = await this.authDb.Groups.Where(g => createRoleDto.GroupIds.Contains(g.Id)).ToListAsync(cancellation);
            }

            var role = new Role()
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                Groups = groups,
            };

            var addedRole = await this.authDb.Roles.AddAsync(role, cancellation) ?? throw new PersistenceException("Failed to add the Role.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new RoleDto(addedRole.Entity);
        }
    }
}
