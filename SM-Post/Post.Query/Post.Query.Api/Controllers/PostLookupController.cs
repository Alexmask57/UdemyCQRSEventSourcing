using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController : Controller
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;


    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts.";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }
    
    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetByPostIdAsync(Guid postId)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery{ Id = postId });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts.";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }
    
    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery{ Author = author });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve posts by author.";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }
    
    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve posts with comments.";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }
    
    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostWithLikesQuery{ NumberOfLikes = numberOfLikes });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve posts with likes.";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        if (posts == null || !posts.Any())
            return NoContent();
        var count = posts.Count;

        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post{(count > 1 ? "s" : String.Empty)}"
        });
    }

    private ActionResult ErrorResponse(Exception ex, string errorMessage)
    {
        _logger.LogError(ex, errorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = errorMessage
        });
    }
}