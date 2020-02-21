using Microsoft.AspNetCore.Identity;

namespace Data.Models.Identity
{
    public class Role : IdentityRole
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}