using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class NewPostCommand:BaseCommand
{
    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;
}