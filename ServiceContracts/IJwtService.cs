using AuthenticationTask.DTO;
using AuthenticationTask.Identity;

namespace AuthenticationTask.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateTwtToken(ApplicationUser user);
    }
}
