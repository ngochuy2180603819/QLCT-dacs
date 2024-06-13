using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
    }
}
