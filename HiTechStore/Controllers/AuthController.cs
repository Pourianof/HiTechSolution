using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public AuthController(UserManager<User> userManager,
                          IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    private async Task<IActionResult> RegisterUser(RegisterDto dto)
    {
        var user = _mapper.Map<User>(dto);
        var result = await _userManager.CreateAsync(user, dto.Password!);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            var problemDetail = new ValidationProblemDetails(ModelState)
            {
                Title = "User registration failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details."
            };
            return BadRequest(problemDetail);
        }

        if (!string.IsNullOrEmpty(dto.Role))
        {
            await _userManager.DeleteAsync(user);
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Title = "User registration failed",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                });
            }
        }

        return Ok(new { Message = "Registering was successful", Status = StatusCodes.Status201Created });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Role))
        {
            var problemDetail = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "You do not have permission to register as roled user.",
                Status = StatusCodes.Status403Forbidden
            };
            return Unauthorized(problemDetail);
        }
        return await RegisterUser(dto);
    }

    [HttpPost("register-by-supervisor")]
    [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
    public async Task<IActionResult> RegisterBySupervisor(RegisterDto dto)
    {
        if (string.IsNullOrEmpty(dto.Role))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Role",
                Detail = "Role is required.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        if (!(User.IsInRole(IdentityRoles.Admin) || User.IsInRole(IdentityRoles.Manager)))
        {
            var problemDetail = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "You do not have permission to register as roled user.",
                Status = StatusCodes.Status403Forbidden
            };
            return Unauthorized(problemDetail);
        }

        return await RegisterUser(dto);
    }

    [HttpPost("login")]
    [TypeFilter(typeof(LoginValidationAttribute))]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var token = HttpContext.Items["Token"];
        return Ok(new { Token = token });
    }

}
