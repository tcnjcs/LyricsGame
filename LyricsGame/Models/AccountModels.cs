using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Web;
using System.Linq;

namespace LyricsGame.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Points { get; set; }
        public string Rank { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveGenre { get; set; }
        public string Picture { get; set; }
    }

    public static class Ranks
    {
        public static void UpdateRank(string username)
        {
            var db = new UsersContext();
            var profiles = db.UserProfiles;

            var user = from p in profiles
                       where p.UserName == username
                       select p;
            var activeUser = user.FirstOrDefault();

            string rank = Ranks.GetRank(activeUser.Points);

            activeUser.Rank = rank;
            db.SaveChanges();
        }
        public static string GetRank(int points)
        {
            if (points >= 1000)
                return "Master Lyricist";
            else if (points >= 500)
                return "Funk Master Flex";
            else if (points >= 100)
                return "Aiight";
            else if (points >= 50)
                return "Not a loser";
            else if (points >= 0)
                return "Loser";

            return "Unranked";
        }

    }
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class FacebookRegisterModel
    {
        public string fbLogin { get; set; }
        public string fbFirst_Name { get; set; }
        public string fbLast_Name { get; set; }
        public string fbProfPic { get; set; }
    }
    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
        public string fbLogin { get; set; }
        public string fbFirst_Name { get; set; }
        public string fbLast_Name { get; set; }
        public string fbProfPic { get; set; }
    }
}
