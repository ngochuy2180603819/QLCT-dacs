namespace Expense_Tracker.Models.Request
{
    public class CategoryRequest
    {
        public string Title { get; set; }
        public string? Icon { get; set; }
        public string Type { get; set; } = "Expense";
    }
}
