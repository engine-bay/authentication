namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class CreateGroup : ICommandHandler<CreateGroupDto, GroupDto>
    {
        private readonly AuthorizationWriteDbContext authDb;
        private readonly IValidator<CreateGroupDto> validator;

        public CreateGroup(
            AuthorizationWriteDbContext authDb,
            IValidator<CreateGroupDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<GroupDto> Handle(CreateGroupDto createGroupDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createGroupDto);

            await this.validator.ValidateAndThrowAsync(createGroupDto, cancellation);

            List<Permission>? permissions = null;
            if (createGroupDto.PermissionIds != null || createGroupDto.PermissionNames != null)
            {
                permissions = await this.authDb.Permissions.Where(p => (createGroupDto.PermissionIds != null && createGroupDto.PermissionIds.Contains(p.Id)) || (createGroupDto.PermissionNames != null && createGroupDto.PermissionNames.Contains(p.Name)))
                    .ToListAsync(cancellation);
            }

            var group = new Group()
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                Permissions = permissions,
            };

            var addedPermission = await this.authDb.Groups.AddAsync(group, cancellation) ??
                                  throw new PersistenceException("Failed to add group.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new GroupDto(addedPermission.Entity);
        }
    }
}
