using System.Text.Json.Serialization;

namespace API.Entities;

public class User
{
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }
    public Role Role { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}