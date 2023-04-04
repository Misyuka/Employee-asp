using System.ComponentModel.DataAnnotations;

namespace Employee.Models.Account
{
	public class DepartmentRegisterViewModel
    {
        public int Number { get; set; }
        public string Manager { get; set; } = null!;
    }
}
