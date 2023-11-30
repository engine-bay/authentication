namespace EngineBay.Authentication
{
    using FluentValidation;

    public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
    {
        public CreateGroupDtoValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}