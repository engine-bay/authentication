namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class CreateAuthUser : ICommandHandler<CreateAuthUserDto, AuthUserDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<CreateAuthUserDto> validator;

        public CreateAuthUser(
            AuthenticationDbContext authDb,
            IValidator<CreateAuthUserDto> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<AuthUserDto> Handle(CreateAuthUserDto createAuthUserDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createAuthUserDto);

            await this.validator.ValidateAndThrowAsync(createAuthUserDto, cancellation);

            List<Role>? roles = null;
            if (createAuthUserDto.RoleIds != null || createAuthUserDto.RoleNames != null)
            {
                roles = await this.authDb.Roles.Where(r => (createAuthUserDto.RoleIds != null && createAuthUserDto.RoleIds.Contains(r.Id)) || (createAuthUserDto.RoleNames != null && createAuthUserDto.RoleNames.Contains(r.Name)))
                    .ToListAsync(cancellation);
            }

            var authUser = new AuthUser()
            {
                ApplicationUserId = createAuthUserDto.UserId,
                Roles = roles,
            };

            var addedUser = await this.authDb.AuthUsers.AddAsync(authUser, cancellation) ??
                            throw new PersistenceException("Did not succesfully add auth user.");
            await this.authDb.SaveChangesAsync(cancellation);

            return new AuthUserDto(addedUser.Entity);
        }
    }
}
