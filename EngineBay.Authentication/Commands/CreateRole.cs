namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;

    public class CreateRole : ICommandHandler<CreateRoleDto, RoleDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreateRoleDto> validator;

        public CreateRole(
            AuthenticationDbContext authDb,
            IValidator<CreateRoleDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<RoleDto> Handle(CreateRoleDto createRoleDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createRoleDto);

            this.validator.ValidateAndThrow(createRoleDto);

            var groups = createRoleDto.Groups?.Select(groupDto => new Group() { Id = groupDto.Id }).ToList();

            if (groups != null)
            {
                this.authDb.Groups.AttachRange(groups);
            }

            var role = new Role()
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                Groups = groups,
            };

            var addedUser = await this.authDb.Roles.AddAsync(role, cancellation) ?? throw new PersistenceException("Did not succesfully add auth user.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new RoleDto(addedUser.Entity);
        }
    }
}
