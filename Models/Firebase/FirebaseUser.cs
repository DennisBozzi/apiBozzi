using System.Security.Claims;

namespace apiBozzi.Models
{
    public class FirebaseUser
    {
        public string? UserId { get; }
        public string? Name { get; }
        public string? Email { get; }
        public string? Picture { get; }
        public bool EmailVerified { get; }
        public bool Admin { get; } = false;

        public FirebaseUser(ClaimsPrincipal user)
        {
            UserId = user.FindFirst("user_id")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Name = user.FindFirst("name")?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value;
            Email = user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;
            Picture = user.FindFirst("picture")?.Value;

            EmailVerified = bool.Parse(user.FindFirst("email_verified")?.Value ?? "false");
            Admin = bool.Parse(user.FindFirst("admin")?.Value ?? "false");
        }
    }
}