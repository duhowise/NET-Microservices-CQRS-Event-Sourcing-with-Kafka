using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class DeletePostCommand:BaseCommand
{
    public string UserName { get; set; }
}