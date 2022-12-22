﻿using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository
{
    Task CreateAsync(PostEntity post);
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid postId);
    Task<PostEntity>GetByIdAsync(Guid postId);
    Task<List<PostEntity>> ListAllAsync();
    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes);
    Task<List<PostEntity>> ListWIthCommentsAsync();

}

public interface ICommentRepository
{
    Task CreateAsync(CommentEntity comment);
    Task UpdateAsync(CommentEntity comment);
    Task DeleteAsync(Guid commentId);
    Task<CommentEntity> GetByIdAsync(Guid commentId);

}