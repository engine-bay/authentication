namespace EngineBay.Authentication
{
    using System;
    using EngineBay.Persistence;

    public class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser applicationUser)
        {
            if (applicationUser is null)
            {
                throw new ArgumentNullException(nameof(applicationUser));
            }

            this.Id = applicationUser.Id;
            this.Username = applicationUser.Username;
        }

        public Guid Id { get; set; }

        public string? Username { get; set; }
    }
}