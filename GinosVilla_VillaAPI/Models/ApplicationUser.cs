using Microsoft.AspNetCore.Identity;

namespace GinosVilla_VillaAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
