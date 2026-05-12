using HiTechStore.Core.Services.Product;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.DTOs.User;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<User> userManager, IProductService productService) : ControllerBase
{
    private UserManager<User> _userManager = userManager;

    [HttpPatch("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateUser(UpdateUserDto updateUserDto)
    {
        var user = (await _userManager.GetUserAsync(User))!;

        if (updateUserDto.FirstName is not null)
        {
            user.FirstName = updateUserDto.FirstName;
        }

        if (updateUserDto.LastName is not null)
        {
            user.LastName = updateUserDto.LastName;
        }

        await _userManager.UpdateAsync(user);


        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? IdentityRoles.User;
        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            // Role = role
        };
    }

    [HttpGet("me/products")]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetMyProducts([ToQuery] ProductQuery productQuery)
    {
        var usersProduct = await productService.GetUsersProducts(productQuery);

        return Ok(usersProduct);
    }
}