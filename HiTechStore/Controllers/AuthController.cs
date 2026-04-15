using System.Security.Claims;

using AutoMapper;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenHandler _tokenHelper;
    private readonly HiTechStore.Core.Services.Authorization.IAuthorizationService _authorizationService;

    public AuthController(UserManager<User> userManager,
                          IMapper mapper,
                          ITokenHandler tokenHelper,
                          HiTechStore.Core.Services.Authorization.IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenHelper = tokenHelper;
        _authorizationService = authorizationService;
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
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
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
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null || !ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        if (loginDto.Email == null && loginDto.Username == null)
        {
            ModelState.AddModelError(
                "Login",
                "Email or Username is required"
            );

            return new BadRequestObjectResult(new ValidationProblemDetails(ModelState)
            {
                Title = "Invalid Login",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Email or Username is required"
            });
        }

        User? user = await _authorizationService.LoginAsync(loginDto);

        if (user == null)
        {
            var authorizationProblemDetail = new ProblemDetails()
            {
                Detail = "Invalid username or password",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized
            };
            return new UnauthorizedObjectResult(authorizationProblemDetail);
        }

        var expiration = DateTime.UtcNow.AddHours(1);
        var token = await _tokenHelper.IssueToken(user.Claims ?? [], user.Id, expiration);

        var authData = new LoginResponseDto
        {
            Token = token.Token,
            ExpiresAt = expiration,
            User = new UserDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles?.Select(r => r.Name!)
            }
        };
        return Ok(authData);
    }

    [HttpGet("refresh")]
    [Authorize]
    public async Task<ActionResult> Refresh([FromQuery] string refreshToken)
    {
        var user = await _authorizationService.GetUserAsync(User.Claims);

        if (user is null)
        {
            return Unauthorized();
        }

        var expiration = DateTime.UtcNow.AddMinutes(30);
        var token = await _tokenHelper.IssueTokenForRefreshToken(refreshToken, User.Claims, expiration);

        return Ok(
            new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiration
            }
        );
    }

    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var user = await _authorizationService.GetUserAsync(User.Claims);

        if (user is null)
        {
            return Unauthorized();
        }

        await _tokenHelper.RevokeRefreshToken(User.Claims);

        return Ok();
    }
}
