using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class YearCharController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public YearCharController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync(DateTime? SelectedDate, String type)
        {
            var userId = _userManager.GetUserId(User);
            DateTime time= new DateTime(DateTime.Today.Year);
            //DateTime StartDate = new DateTime(DateTime.Today.Year,1, 1);
			//DateTime EndDate = StartDate.AddYears(1).AddDays(-1);

            if (SelectedDate.HasValue)
            {
                time = new DateTime(SelectedDate.Value.Year);
                //StartDate = new DateTime(SelectedDate.Value.Year, 1, 1);
                //EndDate = StartDate.AddYears(1).AddDays(-1);
                List<Transaction> selectedTransactions = await _context.Transactions
                        .Include(x => x.Category)
                        //.Where(y => y.UserId == userId && y.Date >= StartDate && y.Date <= EndDate)
                        .Where(y => y.UserId == userId && y.Date.Year == SelectedDate.Value.Year)
                        .ToListAsync();
                if (type == "Expense" || type == "Income")
                {
                    // Prepare data for the doughnut chart
                    ViewBag.DoughnutChartData = selectedTransactions
                        .Where(i => i.Category.Type == type)
                        .GroupBy(j => j.Category.CategoryId)
                        .Select(k => new
                        {
                            categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                            amount = k.Sum(j => j.Amount),
                            formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                        })
                        .OrderByDescending(l => l.amount)
                        .ToList();
                }
                else
                {
                    // Invalid type provided
                    ViewBag.DoughnutChartData = null;
                }

            }


            ViewBag.SelectedDate = SelectedDate;
            ViewBag.Type = type;

            return View();
        }

    }
}