using AutoMapper;

using HiTechStore.ApiTokenHandler.Core;
using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Models;
using HiTechStore.Presentation.Requests.Auth;
using HiTechStore.Presentation.Requests.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Presentation.Auth;
using HiTechStore.Presentation.Responses;
using HiTechStore.Core.Dto.Permission;

namespace HiTechStore.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : AppControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ITokenHandler _tokenHelper;
    private readonly ILogger _logger;
    private readonly Core.Services.Authorization.IAuthorizationService _authorizationService;

    public AuthController(UserManager<User> userManager,
                          IMapper mapper,
                          ITokenHandler tokenHelper,
                          Core.Services.Authorization.IAuthorizationService authorizationService,
                          ILogger<AuthController> logger
                          )
    {
        _userManager = userManager;
        _mapper = mapper;
        _tokenHelper = tokenHelper;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    private async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        var user = _mapper.Map<RegisterDto>(request);
        var result = await _authorizationService.RegisterUser(user);

        return ResultCheck(
            result.WithValue(new { Message = "Registering was successful", Status = StatusCodes.Status201Created }),
            "Registration failed"
        );
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!string.IsNullOrEmpty(request.Role))
        {
            var problemDetail = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "You do not have permission to register as roled user.",
                Status = StatusCodes.Status403Forbidden
            };
            return Unauthorized(problemDetail);
        }
        return await RegisterUser(request);
    }

    [HttpPost("register-by-supervisor")]
    [Authorize(Roles = $"{IdentityRoles.Admin},{IdentityRoles.Manager}")]
    public async Task<IActionResult> RegisterBySupervisor(RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Role))
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

        return await RegisterUser(request);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, [FromServices] IPublicAssetRegisterer assetRegisterer)
    {
        if (loginRequest == null || !ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        if (loginRequest.Email == null && loginRequest.Username == null)
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

        User? user = await _authorizationService.LoginAsync(_mapper.Map<LoginDto>(loginRequest));

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

        _logger.LogInformation(
            "user with id {id} logged-in", user
        );

        var expiration = DateTime.UtcNow.AddMinutes(10); // 10 minute jwt lifetime
        var token = await _tokenHelper.IssueToken(user.Claims ?? [], user.Id, expiration);

        var authData = new LoginResponse
        {
            Token = token.Token,
            RefreshToken = token.RefreshToken,
            ExpiresAt = expiration,
            User = new UserDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles,
                AvatarUrl = user.AvatarUrl is null ? default : assetRegisterer.GetPublicUrl(user.AvatarUrl),
                Permissions = user.Permissions?.Select(up => new UserPermissionDto()
                {
                    Code = up.Permission!.Code,
                    Scope = up.Scope
                }) ?? []
            }
        };
        return Ok(authData);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
    {
        if (dto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(dto.ReturnUrl) || !Uri.TryCreate(dto.ReturnUrl, UriKind.Absolute, out var baseUri))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Return URL",
                Detail = "ReturnUrl must be an absolute URL where the client will display the reset form.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        await _authorizationService.RequestPasswordResetAsync(dto.Email!, token =>
        {
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(dto.Email!);

            var builder = new UriBuilder(baseUri);
            var query = $"token={encodedToken}&email={encodedEmail}";
            if (!string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = builder.Query.TrimStart('?') + "&" + query;
            }
            else
            {
                builder.Query = query;
            }

            return builder.Uri.ToString();
        });

        return Ok(new { Message = "Email with password reset link will send to your email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
    {
        if (dto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authorizationService.ResetPasswordAsync(dto.Email!, dto.Token!, dto.NewPassword!);

        if (result.IsValid && result.Value)
        {
            return Ok(new { Message = "Your password changed succussfully." });
        }

        if (result.Errors != null && result.Errors.OfType<ValidationResultError>().Any())
        {
            return ValidationResult(result.Errors.OfType<ValidationResultError>());
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Reset password failed",
            Detail = string.Join("\n", result.Errors?.Select(err => $"{err.Title}:${err.Description}") ?? []),
            Status = StatusCodes.Status400BadRequest
        });
    }

    [HttpGet("refresh")]
    public async Task<ActionResult> Refresh([FromQuery] string refreshToken)
    {
        var userId = await _tokenHelper.GetRefreshTokenUserId(
            refreshToken
        );

        if (userId is null)
        {
            _logger.LogInformation(
                "could not find user based on ref-token {refToken}", refreshToken
            );

            return Unauthorized();
        }

        var user = await _authorizationService.GetUserByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        var expiration = DateTime.UtcNow.AddMinutes(10);
        var token = await _tokenHelper.IssueTokenForRefreshToken(refreshToken, user.Claims!, expiration);
        _logger.LogInformation(
            "new token issued for user with id {userId}", userId
        );

        Console.WriteLine(expiration);

        return Ok(
            new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiration
            }
        );
    }

    [Route("logout")]
    public async Task<ActionResult> Logout([FromQuery] string refreshToken)
    {
        var userId = await _tokenHelper.GetRefreshTokenUserId(
           refreshToken
       );

        var deletedAny = await _tokenHelper.RevokeRefreshToken(refreshToken);

        _logger.LogInformation(
            "user with id {userId} tried to logout via refresh-token {refToken} and is any token removed: {state}",
            userId,
            refreshToken,
            deletedAny
        );

        return Ok();
    }

    [HttpPatch("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePaswordRequest changePaswordRequest)
    {
        var result = await _authorizationService.ChangePassword(_mapper.Map<ChangePasswordDto>(changePaswordRequest));

        if (result.IsValid && result.Value)
        {
            return Ok(new
            {
                Message = "Password changed successfully"
            });
        }



        return BadRequest(new
        ProblemDetails
        {
            Title = "Password change failed",
            Detail = string.Join(
                "\n",
                result.Errors?.Select(err => $"{err.Title}:${err.Description}") ?? []
            )
        });
    }
}

