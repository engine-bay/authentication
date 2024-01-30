namespace EngineBay.Authentication.Cookies
{
    using FluentValidation;

    public class SignInCredentialsValidator : AbstractValidator<SignInCredentials>
    {
        public SignInCredentialsValidator()
        {
            this.RuleFor(x => x.Username).NotNull().NotEmpty();
            this.RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}