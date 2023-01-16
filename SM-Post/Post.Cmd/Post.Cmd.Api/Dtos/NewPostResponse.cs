using Post.Common.Dtos;

namespace Post.Cmd.Api.Dtos;

public class NewPostResponse:BaseResponse
{
    public Guid Id { get; set; }
}