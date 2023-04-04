namespace Employee.Models
{
    public class HistoryViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; } = null!;
        public string Employee { get; set; } = null!;
        public int DM { get; set; }
        public string State { get; set; } = null!;
    }
}
