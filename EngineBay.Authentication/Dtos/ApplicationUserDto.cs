namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser applicationUser)
        {
            ArgumentNullException.ThrowIfNull(applicationUser);

            this.Id = applicationUser.Id;
            this.Username = applicationUser.Username;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }
    }
}