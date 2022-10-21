using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class AddedMessageCommand:BaseCommand
{
    public string Message { get; set; }
}