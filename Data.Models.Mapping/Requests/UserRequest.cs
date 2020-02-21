using Data.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Mapping.Requests
{
    public class UserRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public bool GeneratePassword { get; set; }
        public string Password { get; set; }

        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}