namespace Employee.Models
{
    public class IndexViewModel
    {
        public List<HistoryViewModel> HistoryInfo { get; set; } = null!;
        public Array employees { get; set; } = null!;
        public Array departments { get; set; } = null!;
        public Array days { get; set; } = null!;
        public int selectedDepartment { get; set; }
        public string selectedYear { get; set; } = null!;
        public string selectedMonth { get; set; } = null!;
    }
}
