namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;

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

            var permissions = createGroupDto.Permissions?.Select(permissionDto => new Permission() { Id = permissionDto.Id }).ToList();

            if (permissions != null)
            {
                this.authDb.Permissions.AttachRange(permissions);
            }

            var group = new Group()
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                Permissions = permissions,
            };

            var addedUser = await this.authDb.Groups.AddAsync(group, cancellation) ?? throw new PersistenceException("Did not succesfully add auth user.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new GroupDto(addedUser.Entity);
        }
    }
}
