using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository:ICommentRepository
{
    private readonly DatabaseContextFactory _factory;

    public CommentRepository(DatabaseContextFactory factory)
    {
        _factory = factory;
    }
    public async Task CreateAsync(CommentEntity comment)
    {
        await using var context = _factory.CreateDbContext();
       await context.Comments.AddAsync(comment);
       _=await context.SaveChangesAsync();

    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        await using var context = _factory.CreateDbContext();
        context.Comments.Update(comment);
        _= await context.SaveChangesAsync();

    }

    public async Task DeleteAsync(Guid commentId)
    {
        await using var context = _factory.CreateDbContext();
        var comment = await GetByIdAsync(commentId);
        if (comment==null)return;
        context.Remove(comment);
        _= await context.SaveChangesAsync();

    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);

    }
}