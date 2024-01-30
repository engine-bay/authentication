namespace EngineBay.Authentication.Cookies
{
    public class SignInCredentials
    {
        public SignInCredentials()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
        }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}