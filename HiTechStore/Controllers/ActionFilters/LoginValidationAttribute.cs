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

                var user = _userManager.FindByNameAsync(loginDto.Username!).Result;
                if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password!).Result)
                {
                    var authorizationProblemDetail = new ProblemDetails() { Detail = "Invalid username or password", Title = "Unauthorized", Status = StatusCodes.Status401Unauthorized };
                    context.Result = new UnauthorizedObjectResult(authorizationProblemDetail);
                    return;
                }

                var token = CreateToken(user);
                Console.WriteLine($"Toke: {token}");
                context.HttpContext.Items["Token"] = token;
                return;
            }
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
            };


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);


            var expiration = DateTime.UtcNow.AddHours(1);

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}