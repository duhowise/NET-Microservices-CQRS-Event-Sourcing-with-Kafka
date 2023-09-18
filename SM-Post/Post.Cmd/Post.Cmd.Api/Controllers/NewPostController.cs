using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Dtos;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController:ControllerBase
{
    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(ILogger<NewPostController>logger,ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
[HttpPost("")]
    public async Task<IActionResult> NewPostAsync(string message, string author)
    {
        var id = Guid.NewGuid();

        try
        {
            var command = new NewPostCommand
            {
                Message = message,
                Author = author,
                Id = id
            };
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse
            {
                Id = id,
                Message = "New post creation request completed successfully"
            });
        }
        catch (InvalidOperationException e)
        {
            _logger.Log(LogLevel.Warning, e, "client made a bad request");
            return BadRequest(new BaseResponse
            {
                Message = e.Message
            });
        }
        catch (Exception exception)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";
            _logger.Log(LogLevel.Error,SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
            {
                Id = id,
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}