using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepositories : IPostRepository
{
    private readonly DatabaseContextFactory _factory;

    public PostRepositories(DatabaseContextFactory factory)
    {
        _factory = factory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        await using var context = _factory.CreateDbContext();
        await context.Posts.AddAsync(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        await using var context = _factory.CreateDbContext();
        context.Posts.Update(post);
        _= await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        await using var context = _factory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post==null)return;
        context.Remove(post);
        _ = await context.SaveChangesAsync();

    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Posts
            .Include(x=>x.Comments)
            .FirstOrDefaultAsync(x=>x.PostId==postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        await using var context = _factory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Posts.AsNoTracking().Include(x => x.Comments)
            .AsNoTracking()
            .Where(x=>x.Author.Contains(author))
            .ToListAsync();

    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Posts.AsNoTracking().Include(x => x.Comments)
            .AsNoTracking()
            .Where(x => x.Likes>=numberOfLikes ).ToListAsync();

    }

    public async Task<List<PostEntity>> ListWIthCommentsAsync()
    {
        await using var context = _factory.CreateDbContext();
        return await context.Posts.AsNoTracking().Include(x => x.Comments)
            .AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any()).ToListAsync();
    }
}