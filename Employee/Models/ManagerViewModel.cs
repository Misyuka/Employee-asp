namespace Employee.Models
{
    public class ManagerViewModel
    {
        public List<HistoryViewModel> HistoryInfo { get; set; } = null!;
        public Array employees { get; set; } = null!;
        public Array days { get; set; } = null!;
    }
}
