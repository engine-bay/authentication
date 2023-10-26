namespace EngineBay.Authentication
{
    public class CreateUserDto
    {
        public CreateUserDto(string username)
        {
            this.Username = username;
        }

        public string Username { get; set; }
    }
}