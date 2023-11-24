namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class CreatePermission : ICommandHandler<CreatePermissionDto, PermissionDto>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreatePermissionDto> validator;

        public CreatePermission(
            AuthenticationDbContext authDb,
            IValidator<CreatePermissionDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<PermissionDto> Handle(CreatePermissionDto createPermissionDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createPermissionDto);

            this.validator.ValidateAndThrow(createPermissionDto);

            var permission = new Permission()
            {
                Name = createPermissionDto.Name,
            };

            var addedUser = await this.authDb.Permissions.AddAsync(permission, cancellation) ?? throw new PersistenceException("Did not succesfully add auth user.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new PermissionDto(addedUser.Entity);
        }
    }
}
