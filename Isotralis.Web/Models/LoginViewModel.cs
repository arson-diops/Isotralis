using System.ComponentModel.DataAnnotations;

namespace Isotralis.Web.Models;

public sealed record LoginViewModel
{
    [Required(ErrorMessage = "Enter your username.")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    public string Password { get; init; } = string.Empty;
}