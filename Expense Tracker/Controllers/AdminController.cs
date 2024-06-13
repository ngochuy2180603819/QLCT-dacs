using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
	public class AdminController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> IndexAsync()
		{
			var users = await _userManager.Users.ToListAsync();
            var nonAdminUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }
            // Pass the list of users to the view
            return View(nonAdminUsers);
		}

		public async Task<IActionResult> Delete(String id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return View("NotFound");
			return View(user);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(String id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return View("NotFound");

			await _userManager.DeleteAsync(user);
			return RedirectToAction(nameof(Index));
		}

	}
}