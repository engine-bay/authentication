namespace EngineBay.Authentication
{
    using EngineBay.Persistence;

    public class UnauthenticatedUser : ApplicationUser
    {
        public UnauthenticatedUser()
            : base(DefaultAuthenticationConfigurationConstants.UnauthenticatedUserName)
        {
            this.Id = Guid.Empty;
        }
    }
}
