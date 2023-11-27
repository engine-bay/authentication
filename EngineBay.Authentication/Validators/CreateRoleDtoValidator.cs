namespace EngineBay.Authentication
{
    using FluentValidation;

    public class CreateRoleDtoValidator : AbstractValidator<CreateOrUpdateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}