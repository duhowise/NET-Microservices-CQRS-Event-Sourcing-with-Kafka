using CQRS.Core.Commands;
using Mediator;

namespace Post.Cmd.Api.Commands;

public class AddedMessageCommand:BaseCommand,ICommand
{
    public string Message { get; set; }
}