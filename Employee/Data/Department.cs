namespace Employee.Data
{
    public class Department
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Manager { get; set; } = null!;
        public string State { get; set; } = null!;
    }
}
