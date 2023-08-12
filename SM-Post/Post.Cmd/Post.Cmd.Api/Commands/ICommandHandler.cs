using Mediator;

namespace Post.Cmd.Api.Commands;

public interface ICommandHandler:
    ICommandHandler<NewPostCommand>,
    ICommandHandler<EditMessageCommand>
    ,ICommandHandler<LikePostCommand>
    ,ICommandHandler<AddCommentCommand>
    ,ICommandHandler<EditCommentCommand>
    ,ICommandHandler<RemoveCommentCommand>
    ,ICommandHandler<DeletePostCommand>
{
}