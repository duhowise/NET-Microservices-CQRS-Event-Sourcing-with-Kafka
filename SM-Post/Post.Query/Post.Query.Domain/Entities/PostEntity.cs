﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Post.Query.Domain.Entities;
[Table("Post")]
public class PostEntity
{
    public Guid PostId { get; set; }
    public string Author { get; set; }
    public DateTime DatePosted { get; set; }
    public int Likes { get; set; }
    public string Message { get; set; }
    [JsonIgnore]
    public virtual ICollection<CommentEntity> Comments { get; set; }
}