namespace Holocron.App.Api.Data.Entities;

public class LikeEntity : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = null!;
    public string Name { get; set; } = null!;

    public LikeEntity(string name)
    {
        Name = name;
    }

    // Required for EF
    protected LikeEntity() { }
}
