namespace EngineBay.Authentication
{
    public class CreateAuthUserDto
    {
        public CreateAuthUserDto(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; set; }

        public ICollection<Guid>? RoleIds { get; set; }
    }
}