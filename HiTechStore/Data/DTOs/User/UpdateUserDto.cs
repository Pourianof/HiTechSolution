using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.User;


public class UpdateUserDto
{
    [MinLength(2)]
    public string? FirstName { get; set; }
    [MinLength(2)]
    public string? LastName { get; set; }
}