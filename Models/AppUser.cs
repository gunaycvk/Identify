using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Models
{

    public class AppUser : IdentityUser
    {

        public String? FullName { get; set; }

    }


}