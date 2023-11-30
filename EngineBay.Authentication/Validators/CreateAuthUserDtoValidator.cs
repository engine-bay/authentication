namespace EngineBay.Authentication
{
    using FluentValidation;

    public class CreateAuthUserDtoValidator : AbstractValidator<CreateAuthUserDto>
    {
        public CreateAuthUserDtoValidator()
        {
            this.RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}