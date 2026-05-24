using Microsoft.AspNetCore.Identity;

namespace MotoAdvisor.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<UserFavorite> Favorites { get; set; } = [];
}
