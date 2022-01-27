using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.IdentityServer.Model.Context
{
    public class MySQLContext : IdentityDbContext<ApplicationUser>
    {
        public MySQLContext(DbContextOptions<MySQLContext> options)
            : base(options) { }
    }
}
