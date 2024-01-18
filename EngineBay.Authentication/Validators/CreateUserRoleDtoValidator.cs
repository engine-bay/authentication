namespace EngineBay.Authentication
{
    using FluentValidation;

    public class CreateUserRoleDtoValidator : AbstractValidator<CreateUserRoleDto>
    {
        public CreateUserRoleDtoValidator()
        {
            this.RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}