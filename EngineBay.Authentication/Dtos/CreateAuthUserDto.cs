namespace EngineBay.Authentication
{
    public class CreateAuthUserDto
    {
        public CreateAuthUserDto(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; set; }

        public ICollection<RoleDto>? Roles { get; set; }
    }
}