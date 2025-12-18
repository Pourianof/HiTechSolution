using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace HiTechStore.Controllers.ActionFilters
{
    public class LoginValidationAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public LoginValidationAttribute(IConfiguration configuration,
                        UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("dto"))
            {
                var loginDto = context.ActionArguments["dto"] as LoginDto;
                if (loginDto == null || !context.ModelState.IsValid)
                {
                    context.Result = new BadRequestObjectResult(context.ModelState);
                    return;
                }
                else if (loginDto.Email == null && loginDto.Username == null)
                {
                    context.ModelState.AddModelError(
                        "Login",
                        "Email or Username is required"
                    );

                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState)
                    {
                        Title = "Invalid Login",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Email or Username is required"
                    });

                    return;
                }

                User? user;
                if (loginDto.Email is null)
                {
                    user = _userManager.FindByNameAsync(loginDto.Username!).Result;
                }
                else
                {
                    user = _userManager.FindByEmailAsync(loginDto.Email!).Result;
                }

                if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password!).Result)
                {
                    var authorizationProblemDetail = new ProblemDetails() { Detail = "Invalid username or password", Title = "Unauthorized", Status = StatusCodes.Status401Unauthorized };
                    context.Result = new UnauthorizedObjectResult(authorizationProblemDetail);
                    return;
                }

                var roles = _userManager.GetRolesAsync(user).Result;

                var expiration = DateTime.UtcNow.AddHours(1);
                var role = roles.FirstOrDefault() ?? IdentityRoles.User;
                var token = CreateToken(user, expiration, role);
                context.HttpContext.Items["AuthData"] = new LoginResponseDto
                {
                    Token = token,
                    ExpiresAt = expiration,
                    User = new UserDto
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = role
                    }
                };
                return;
            }
        }

        private string CreateToken(User user, DateTime expiration, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, role)
            };


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds,
                audience: _configuration["Jwt:Audience"],
                issuer: _configuration["Jwt:Issuer"]
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}