using Microsoft.AspNetCore.Identity;

namespace SecureBank.Service.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
}
