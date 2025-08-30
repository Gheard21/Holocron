namespace Holocron.App.Api.Data.Entities;

public class CommentEntity : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime DateWatched { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = null!;
}
