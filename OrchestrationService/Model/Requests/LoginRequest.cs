using System.ComponentModel.DataAnnotations;

namespace OrchestrationService.Model.Requests;

public class LoginRequest
{
        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
}