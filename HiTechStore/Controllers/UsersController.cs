using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Data.DTOs.User;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HiTechStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<User> userManager) : ControllerBase
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
}