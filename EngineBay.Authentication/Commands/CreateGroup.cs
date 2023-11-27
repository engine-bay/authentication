namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class CreateGroup : ICommandHandler<CreateGroupDto, GroupDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreateGroupDto> validator;

        public CreateGroup(
            AuthenticationDbContext authDb,
            IValidator<CreateGroupDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<GroupDto> Handle(CreateGroupDto createGroupDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createGroupDto);

            this.validator.ValidateAndThrow(createGroupDto);

            // Need to make same decision as for create role, but I think in this case it's more likely for the models to already be in the context since this will be used during the database seeding process
            List<Permission>? permissions = null;
            if (createGroupDto.PermissionIds != null)
            {
                permissions = await this.authDb.Permissions.Where(p => createGroupDto.PermissionIds.Contains(p.Id)).ToListAsync(cancellation);
            }

            var group = new Group()
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                Permissions = permissions,
            };

            var addedPermission = await this.authDb.Groups.AddAsync(group, cancellation) ?? throw new PersistenceException("Failed to add group.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new GroupDto(addedPermission.Entity);
        }
    }
}
