using Microsoft.AspNetCore.Identity;

namespace ITS_System.Areas.Admin.Models
{
    public class ManageUserRolesViewModel
    {
        public IdentityUser User { get; set; }

        public IdentityRole Role { get; set; }

        public bool InRole { get; set; }
    }
}
