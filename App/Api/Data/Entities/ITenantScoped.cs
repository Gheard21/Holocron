namespace Holocron.App.Api.Data.Entities;

// Marker contract for entities that have a TenantId set automatically
public interface ITenantScoped
{
    string TenantId { get; set; }
}
