using Employee.Data;
using Employee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Controllers
{
    public class ManagerController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public ManagerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}
		public IActionResult Index()
        {
            List<HistoryViewModel> historyViewModels = new List<HistoryViewModel>();
            var month = DateTime.Now.ToString("MM");
            var year = DateTime.Now.ToString("yyyy");
            var time = month + "." + year;
            var authedUser = User.Identity.Name;
            var userData = _userManager.Users.Where(e => e.UserName == authedUser).FirstOrDefault();
            var historiesTemp = db.Histories.Where(e => e.Date.Contains(time) && e.DM == userData.DM).ToList();
            foreach (var history in historiesTemp)
            {
                historyViewModels.Add(new HistoryViewModel()
                {
                    Id = history.Id,
                    Date = history.Date,
                    Employee = history.Employee,
                    DM = history.DM,
                    State = history.State
                });
            }
            var data = db.Employees.Where(e => e.DM == userData.DM).ToArray();
            string[] employees = new string[data.Length];
            var i = 0;
            foreach (var e in data)
            {
                employees[i] = e.Name;
                i++;
            }
            int[] months = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var checkYear = DateTime.Now.Year;
            if (checkYear % 4 == 0 && (checkYear % 100 != 0 || checkYear % 400 == 0))
            {
                months[2] = 29;
            }
            string[] days = new string[months[Convert.ToInt32(month)]];
            var day = "";
            for (var j = 1; j <= days.Length; j++)
            {
                if (j < 10)
                {
                    day = "0" + j + "." + month + "." + year;
                }
                else
                {
                    day = j + "." + month + "." + year;
                }
                days[j - 1] = day;
            }

            ManagerViewModel model = new ManagerViewModel();
            model.HistoryInfo = historyViewModels;
            model.employees = employees;
            model.days = days;
            return View(model);
        }
        [HttpPost]
        public void CreateHistory(History model)
        {
			var authedUser = User.Identity.Name;
			var userData = _userManager.Users.Where(e => e.UserName == authedUser).FirstOrDefault();
			History history = new History();
            history.Date = model.Date;
            history.Employee = model.Employee;
            history.DM = userData.DM;
            history.State = model.State;
            db.Histories.Add(history);
            db.SaveChanges();
        }
    }
}
