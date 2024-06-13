﻿using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class MonthCharController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MonthCharController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
            _userManager = userManager;
        }

		public async Task<IActionResult> IndexAsync(DateTime? SelectedDate, String type)
		{
            var userId = _userManager.GetUserId(User);
            DateTime StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			DateTime EndDate = StartDate.AddMonths(1).AddDays(-1);

			if (SelectedDate.HasValue)
			{
				System.Console.WriteLine("value" + SelectedDate.Value.ToString());
				StartDate = new DateTime(SelectedDate.Value.Year, SelectedDate.Value.Month, 1);
				EndDate = StartDate.AddMonths(1).AddDays(-1);

				List<Transaction> selectedTransactions = await _context.Transactions
					.Include(x => x.Category)
					.Where(y => y.UserId == userId && y.Date >= StartDate && y.Date <= EndDate)
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