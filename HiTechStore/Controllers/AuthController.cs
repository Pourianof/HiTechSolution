using AutoMapper;

using HiTechStore.Controllers.ActionFilters;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
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

        return Ok(new { Message = "Registering was successful", Status = StatusCodes.Status201Created });
    }

    [HttpPost("login")]
    [TypeFilter(typeof(LoginValidationAttribute))]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var token = HttpContext.Items["Token"];
        return Ok(new { Token = token });
    }

}
