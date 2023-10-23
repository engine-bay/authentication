namespace EngineBay.Authentication
{
    using EngineBay.Persistence;

    public class SystemUser : ApplicationUser
    {
        public SystemUser()
            : base(DefaultAuthenticationConfigurationConstants.SystemUserName)
        {
            this.Id = Guid.Empty;
        }
    }
}
