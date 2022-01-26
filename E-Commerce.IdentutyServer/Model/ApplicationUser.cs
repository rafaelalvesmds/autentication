using Microsoft.AspNetCore.Identity;

namespace E_Commerce.IdentutyServer.Model
{
    public class ApplicationUser : IdentityUser
    {
        private string  FirstName { get; set; }
        private string SecondName { get; set; }
    }
}
