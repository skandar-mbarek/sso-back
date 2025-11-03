using System.ComponentModel.DataAnnotations;

namespace sso_back.Entities;

public class BaseEntity
{
    [Key]
    public string Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}