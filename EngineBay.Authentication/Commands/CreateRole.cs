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

            // Decide on an option before/during review:
            // Attaching a model will give them the "Unchanged" tracking status. So long as it isn't updated later in this DbContext (which should be scoped only to this request), the group model won't be updated when changes are saved.
            // var groups = createRoleDto.GroupIds?.Select(id => new Group() { Id = id }).ToList();
            // if (groups != null)
            // {
            //    this.authDb.Groups.AttachRange(groups);
            // }

            // A safer but slower alternative is to load the groups from the DB
            List<Group>? groups = null;
            if (createRoleDto.GroupIds != null)
            {
                groups = await this.authDb.Groups.Where(g => createRoleDto.GroupIds.Contains(g.Id)).ToListAsync(cancellation);
            }

            // Could use FindAsync if we're expecting them to already be in the DB Context, but I don't thinkg they'd be
            // if (createRoleDto.GroupIds != null)
            // {
            //    groups = new List<Group>();
            //    foreach (var groupId in createRoleDto.GroupIds)
            //    {
            //        var group = await this.authDb.Groups.FindAsync(new object[] { groupId }, cancellationToken: cancellation);
            //        if (group != null)
            //        {
            //            groups.Add(group);
            //        }
            //    }
            // }
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
