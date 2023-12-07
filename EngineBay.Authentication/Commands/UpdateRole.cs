namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class UpdateRole : ICommandHandler<UpdateRoleCommand, RoleDto>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly IValidator<UpdateRoleCommand> validator;

        public UpdateRole(
            AuthenticationDbContext authDb,
            IValidator<UpdateRoleCommand> validator)
        {
            this.authDb = authDb;
            this.validator = validator;
        }

        public async Task<RoleDto> Handle(UpdateRoleCommand updateRoleCommand, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(updateRoleCommand);

            await this.validator.ValidateAndThrowAsync(updateRoleCommand, cancellation);

            List<Group>? groups = null;
            if (updateRoleCommand.GroupIds != null || updateRoleCommand.GroupNames != null)
            {
                groups = await this.authDb.Groups.Where(g => (updateRoleCommand.GroupIds != null && updateRoleCommand.GroupIds.Contains(g.Id)) || (updateRoleCommand.GroupNames != null && updateRoleCommand.GroupNames.Contains(g.Name)))
                    .ToListAsync(cancellation);
            }

            var role = await this.authDb.Roles.FindAsync(new object[] { updateRoleCommand.Id }, cancellationToken: cancellation) ?? throw new NotFoundException($"No Role with Id ${updateRoleCommand.Id} found.");

            role.Name = updateRoleCommand.Name;
            role.Description = updateRoleCommand.Description;
            role.Groups = groups;

            var updatedRole = this.authDb.Roles.Update(role);
            await this.authDb.SaveChangesAsync(cancellation);

            return new RoleDto(updatedRole.Entity);
        }
    }
}
