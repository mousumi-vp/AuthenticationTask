using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationTask.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage="Person Name can't be blank")]
        public string PersoneName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in proper format")]
        [Remote(action:"IsEmailAlreadyRegister",controller: "Account",
            ErrorMessage ="Email is already in use")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone Number can't be blank")]
        [RegularExpression("^[0-9]*$",ErrorMessage = "Phone Number can't be blank")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirm Password can't be blank")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
