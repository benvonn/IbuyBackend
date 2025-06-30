using Ibuy.Service;
using Ibuy.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ibuy.Models
{

    public class UserFromRequest
    {
        public string Username { get; set; } = null!;
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PreferredContact { get; set; }

    }



    public static class UserExtensions
    {
        public static User GetUser(this UserFromRequest userRequest, PasswordServices passwordService)
        {
            return new User { Username = userRequest.Username, Email = userRequest.Email, PasswordHash = passwordService.HashPassword(userRequest.Password), PreferredContact = userRequest.PreferredContact };
        }
    }

    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string PreferredContact { get; set; }

    }

    public class ContactUpdateRequest
    {
        public int UserId { get; set; }
        public string PreferredContact { get; set; }
    }
}
