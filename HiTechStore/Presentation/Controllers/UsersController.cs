using HiTechStore.Core.Services.Product;
using HiTechStore.Core.Services.UserService;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.DTOs.Authorization;
using HiTechStore.Infrastructure.Data.DTOs.Product;
using HiTechStore.Infrastructure.Data.DTOs.User;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<User> userManager, IProductService productService, IUserService userService) : ControllerBase
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
            AvatarUrl = user.AvatarUrl
            // Role = role
        };
    }

    [HttpGet("me/products")]
    [Authorize]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetMyProducts([ToQuery] ProductQuery productQuery)
    {
        var usersProduct = await productService.GetUsersProducts(productQuery);

        return Ok(usersProduct);
    }

    [HttpPut("me/avatar")]
    [Authorize]
    public async Task<ActionResult> UpdateProfileAvatar(IFormFile avatar)
    {
        var newAvatarUrl = await userService.UpdateProfileAvatar(avatar);

        return Ok(
            new UserDto
            {
                AvatarUrl = newAvatarUrl
            }
        );
    }
}