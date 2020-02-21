using Data.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Mapping.Requests
{
    public class RegisterRequest
    {
        #region Required

        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(9)]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(3)]
        public string Password { get; set; }

        #endregion Required

        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
    }

    public class UserUpdateRequest
    {
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }

        #region Advertising

        public bool IsSmsAdvertisingPermitted { get; set; }
        public bool IsEmailAdvertisingPermitted { get; set; }

        #endregion Advertising
    }
}