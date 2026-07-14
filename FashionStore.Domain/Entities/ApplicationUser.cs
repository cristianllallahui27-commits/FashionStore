using Microsoft.AspNetCore.Identity;

namespace FashionStore.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool Activo { get; set; } = true;
    }
}
