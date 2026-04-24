using HiTechStore.Core.Services.Comment;
using HiTechStore.Models;
using HiTechStore.Presentation.Requests.Comment;

using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpGet("{commentId}")]
    public async Task<ActionResult<Comment>> GetSingleComment(int commentId)
    {
        var comment = await commentService.GetCommentById(commentId);

        return Ok(comment);
    }

    [HttpPost("{commentId}/responses")]
    public async Task<ActionResult<Comment>> ResponseToComment(int commentId, ResponseCommentRequest responseComment)
    {
        var comment = await commentService.GetCommentById(commentId);

        return Ok(comment);
    }
}