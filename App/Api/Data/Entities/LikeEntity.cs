namespace Holocron.App.Api.Data.Entities;

public class LikeEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = null!;
    public string Name { get; set; }

    public LikeEntity(string name)
    {
        Name = name;
    }

    // Required for EF
    protected LikeEntity()
    {
        Name = null!;
    }
}
