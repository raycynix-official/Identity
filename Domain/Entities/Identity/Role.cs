using Microsoft.AspNetCore.Identity;

namespace Raycynix.Services.AuthService.Domain.Entities.Identity;

public sealed class Role : IdentityRole<Guid>
{
    public Role()
    {
    }

    public Role(string name) : base(name)
    {
    }

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpper();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}