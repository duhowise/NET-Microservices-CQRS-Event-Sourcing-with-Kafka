using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostLikedEventHandler : IEventHandler<PostLikedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostLikedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await _postRepository.GetByIdAsync(@event.Id);
        if (post == null) return;
        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }
}