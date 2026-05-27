using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Use the domain ApplicationUser type to avoid duplicate entity types with the same
// name in different namespaces. This ensures the Identity DbContext uses the
// same ApplicationUser entity defined in the Domain project.
public class FashionStoreWebContext(DbContextOptions<FashionStoreWebContext> options)
    : IdentityDbContext<FashionStore.Domain.Entities.ApplicationUser>(options)
{
}
