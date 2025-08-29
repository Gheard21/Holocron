namespace Holocron.App.Api.Models.Requests;

public class NewCommentRequest
{
    public string Name { get; set; } = null!;
    public DateTime DateWatched { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = null!;
}