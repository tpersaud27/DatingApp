using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public DateOnly? DateOfBirth { get; set; } // Optional to make required work!

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your Password is limited to {2} to {1} characters",
            MinimumLength = 6)]
        public string Password { get; set; }


    }
}
