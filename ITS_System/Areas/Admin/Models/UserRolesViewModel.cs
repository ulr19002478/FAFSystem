using Microsoft.AspNetCore.Identity;

namespace ITS_System.Areas.Admin.Models
{
    public class UserRolesViewModel
    {
        public IdentityUser User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
