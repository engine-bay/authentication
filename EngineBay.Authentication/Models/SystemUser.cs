namespace EngineBay.Authentication
{
    using EngineBay.Persistence;

    public class SystemUser : ApplicationUser
    {
        public SystemUser()
        {
            this.Id = Guid.Empty;
            this.Name = "System";
        }
    }
}
