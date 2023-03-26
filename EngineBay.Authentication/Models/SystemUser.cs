namespace EngineBay.Authentication
{
    using EngineBay.Persistence;

    public class SystemUser : ApplicationUser
    {
        public SystemUser()
        {
            this.Id = default(Guid);
            this.Username = DefaultAuthenticationConfigurationConstants.SystemUserName;
        }
    }
}
