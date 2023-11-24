namespace EngineBay.Authentication
{
    public class CreatePermissionDto
    {
        public CreatePermissionDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}