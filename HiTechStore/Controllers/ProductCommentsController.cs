using AutoMapper;

using HiTechStore.Core.Dto.ProductComment;
using HiTechStore.Core.Services.ProductComment;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Presentation.Requests.Comment;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/products/{productId}/comments")]
public class ProductCommentsController(
    IProductCommentService productCommentService,
    IMapper mapper
) : ControllerBase
{

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> RegisterCommentForProduct(int productId, CommentCreationRequest commentDto)
    {
        var comment = await productCommentService.AddCommentForProduct(productId, mapper.Map<ProductCommentCreationDto>(commentDto));

        return Ok(comment);
    }

    [HttpGet]
    public async Task<ActionResult> GetCommentsOfProduct(int productId, [ToQuery] BaseQuery query)
    {
        var comments = await productCommentService.GetProductComments(productId, query);

        return Ok(comments);
    }
}