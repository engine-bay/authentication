namespace EngineBay.Authentication
{
    using FluentValidation;

    public class CreatePermissionDtoValidator : AbstractValidator<CreatePermissionDto>
    {
        public CreatePermissionDtoValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}