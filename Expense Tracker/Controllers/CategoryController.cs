using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var categories = await _context.Categories
                                          .Where(c => c.UserId == userId)
                                          .ToListAsync();
            return categories != null ?
                View(categories) :
                Problem("Entity set 'ApplicationDbContext.Categories' is null.");
        }


        // GET: Category/AddOrEdit
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;
            if (id == 0)
                return View(new Category { UserId = userId });
            else
                return View(await _context.Categories.FindAsync(id));
        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            ModelState.Remove(nameof(Category.UserId));
            ModelState.Remove(nameof(Category.CategoryId));
            ModelState.Remove(nameof(Category.User));

            var userId = _userManager.GetUserId(User);

            category.UserId = userId;
            if (ModelState.IsValid)
            {

                if (category.CategoryId == 0)
                {
                    var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Title == category.Title && c.UserId == userId);
                    if (existingCategory != null)
                    {
                        ModelState.AddModelError("", "Title already exists");
                        return View(category);
                    }
                    _context.Add(category);
                }
                else
                {
					var existingCategory = await _context.Categories
				   .FirstOrDefaultAsync(c => c.Title == category.Title && c.UserId == userId && c.CategoryId != category.CategoryId);
					if (existingCategory != null)
					{
						ModelState.AddModelError("", "Title already exists");
						return View(category);
					}
					_context.Update(category);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
