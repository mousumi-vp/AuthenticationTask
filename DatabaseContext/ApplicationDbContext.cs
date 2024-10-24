using AuthenticationTask.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationTask.DatabaseContext
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {

        }

        public ApplicationDbContext()
        {

        }
    }
}
