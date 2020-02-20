using System.ComponentModel.DataAnnotations;

namespace Data.Models.Mapping.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [Compare("NewPasswordAgain")]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordAgain { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [MinLength(3)]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [MinLength(3)]
        public string Email { get; set; }
        [Required]
        [MinLength(3)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(3)]
        public string Token { get; set; }
    }
}
