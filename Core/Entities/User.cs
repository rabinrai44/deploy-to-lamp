using System.Text.Json.Serialization;

namespace deploy_to_linux.Core.Entities.Entities;

public class User
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }
    public Role Role { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}