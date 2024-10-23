using Microsoft.AspNetCore.Identity;

namespace AuthenticationTask.Identity
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
    }
}
