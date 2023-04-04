using Employee.Data;
using Employee.Models;
using Employee.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;using System.IO;
using System.Text;
using iText.Html2pdf;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace Employee.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public ActionResult Index(string year, string month, int DM)
        {
            if (year == null)
            {
                month = DateTime.Now.ToString("MM");
                year = DateTime.Now.ToString("yyyy");
            }
            var time = month + "." + year;

            if (DM == 0)
            {
                DM = 1;
            }
            List<HistoryViewModel> historyViewModels = new List<HistoryViewModel>();
            var dm = db.Departments.Where(e => e.State == "Enable").ToArray();

            int[] departments = new int[dm.Length];
            var i = 0;
            foreach (var d in dm)
            {
                departments[i] = d.Number;
                i++;
            }
            var historiesTemp = db.Histories.Where(e => e.Date.Contains(time) && e.DM == DM).ToList();
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
            var data = db.Employees.Where(e => e.DM == DM).ToArray();
			string[] employees = new string[data.Length];
			i = 0;
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
            for (var j = 1;  j <= days.Length; j++)
            {
                if(j < 10)
                {
                    day = "0" + j + "." + month + "." + year;
                } else
                {
                    day =   j + "." + month + "." + year;
                }
                days[j - 1] = day;
            }

            IndexViewModel model = new IndexViewModel();
            model.HistoryInfo = historyViewModels;
            model.employees = employees;
            model.departments = departments;
            model.selectedDepartment = DM;
            model.selectedYear = year;
            model.selectedMonth = month;
            model.days = days;

            return View(model);
        }
        public IActionResult Department()
        {
            var departments = db.Departments.ToList();
            return View(departments);
        }
		[HttpPost]
		public ActionResult DepartmentRegister(DepartmentRegisterViewModel model)
        {
			Department department = new Department();
            department.Number = model.Number;
            department.Manager = model.Manager;
            department.State = "Enable";
			db.Departments.Add(department);
			db.SaveChanges();
			return RedirectToAction("Department", "Home");
		}
        public IActionResult DepartmentRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditDM(EditDepartmentViewModel model)
        {
            Department dm = db.Departments.Where(x => x.Number == model.Number).FirstOrDefault();
            dm.Number = model.Number;
            dm.Manager = model.Manager;
            dm.State = model.State;
            db.SaveChanges();
            return RedirectToAction("Department", "Home");
        }
        [HttpPost]
        public ActionResult DeleteDM(int number)
        {
            db.Remove(db.Departments.Where(x => x.Number == number).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Department", "Home");
        }
        public IActionResult DMManager()
        {
            var managers = _userManager.Users.Where(e => e.Role == 1).ToList();
            return View(managers);
        }
        [HttpPost]
        public async Task<IActionResult> DMManagerRegister(DMMRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.UserName,
                    EmailConfirmed = true,
                    DM = model.DM,
                    Role = 1
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("DMManager", "Home"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(model);
        }
        public ActionResult DMManagerRegister() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditManager(EditManagerViewModel model)
        {
            ApplicationUser manager = _userManager.Users.Where(x => x.Id == model.Id).FirstOrDefault();
            manager.Email = model.username;
            manager.DM = model.dm;

            await _userManager.UpdateAsync(manager);

            return RedirectToAction("DMManager", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteManager(string id)
        {
            ApplicationUser manager = _userManager.Users.Where(x => x.Id == id).FirstOrDefault();
            await _userManager.DeleteAsync(manager);
            return RedirectToAction("DMManager", "Home");
        }
        public IActionResult Employee()
        {
            var employees = db.Employees.ToList();
            return View(employees);
        }
        [HttpPost]
		public ActionResult EmployeeRegister(EmployeeRegisterViewModel model)
		{
            EmployeeInfo employee = new EmployeeInfo();
            employee.Name = model.Name;
            employee.DM = model.DM;
            db.Employees.Add(employee);
            db.SaveChanges();
            return RedirectToAction("Employee", "Home");
		}
        public IActionResult EmployeeRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditEmployee(EditEmployeeViewModel model)
        {
            EmployeeInfo employee = db.Employees.Where(x => x.Id == model.id).FirstOrDefault();
            employee.Name = model.name;
            employee.DM = model.dm;
            db.SaveChanges();
            return RedirectToAction("Employee", "Home");
        }
        [HttpPost]
        public ActionResult DeleteEmployee(int id)
        {
            db.Remove(db.Employees.Where(x => x.Id == id).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("Employee", "Home");
        }
        [HttpPost]
        public void CreateHistory(History model)
        {
            History history = new History();
            history.Date = model.Date;
            history.Employee = model.Employee;
            history.DM = model.DM;
            history.State = model.State;
            db.Histories.Add(history);
            db.SaveChanges();
        }
        [HttpPost]
        public void UpdateHistory(UpdateHistoryViewModel model)
        {
            History history = db.Histories.Where(x => x.Id == model.Id).FirstOrDefault();
            history.State = model.State;
            db.SaveChanges();
        }
        [HttpPost]
        public void DeleteHistory(int id)
        {
            db.Remove(db.Histories.Where(x => x.Id == id).FirstOrDefault());
            db.SaveChanges();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public ActionResult ExportYearReport(string year, int DM)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<h3>" + year + "Year Report(Department - " + DM + ") </h3>");
            var data = db.Employees.Where(e => e.DM == DM).ToArray();
            string[] employees = new string[data.Length];
            var i = 0;
            foreach (var e in data)
            {
                employees[i] = e.Name;
                i++;
            }
            var historiesTemp = db.Histories.Where(e => e.Date.Contains(year) && e.DM == DM).ToList();
            int[] months = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var checkYear = DateTime.Now.Year;
            if (checkYear % 4 == 0 && (checkYear % 100 != 0 || checkYear % 400 == 0))
            {
                months[2] = 29;
            }
            string[] monthArray = new string[12] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            foreach (var k in monthArray)
            {
                sb.Append("<div>" +k+ "</div><table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #d9d9d9; font-size: 6pt; margin-bottom: 20px;'><thead><tr><th style='background-color: #bdd7ee;'>Employees</th>");

                string[] days = new string[months[Convert.ToInt32(k)]];
                var day = "";
                for (var j = 1; j <= days.Length; j++)
                {
                    if (j < 10)
                    {
                        day = "0" + j + "." + k + "." + year;
                    }
                    else
                    {
                        day = j + "." + k + "." + year;
                    }
                    days[j - 1] = day;
                    sb.Append("<th style='background-color: #ffe699; min-width: 8px;'>" + j + "</th>");
                }
                sb.Append("<th>H</th><th>S</th></tr></thead><tbody>");
                foreach (var employee in employees)
                {
                    sb.Append("<tr><td>" + employee + "</td>");
                    var h = 0;
                    var s = 0;
                    foreach (var item in days)
                    {
                        var state = false;
                        var info = "";
                        string[] split = item.Split('.');
                        var date = new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]));
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            sb.Append("<td style='background-color: #ededed;'></td>");
                        }
                        else
                        {
                            foreach (var history in historiesTemp)
                            {
                                if (employee.ToString() == history.Employee.ToString() && item.ToString() == history.Date.ToString())
                                {
                                    state = true;
                                    info = history.State;
                                }
                            }
                            if (state == true)
                            {
                                if (info == "H")
                                {
                                    h++;
                                    sb.Append("<td style='background-color: #c6e0b4;'>" + info + "</td>");
                                }
                                else
                                {
                                    s++;
                                    sb.Append("<td style='background-color: #fce4d6;'>" + info + "</td>");
                                }
                            }
                            else
                            {
                                sb.Append("<td></td>");
                            }
                        }
                    }
                    sb.Append("<td>" + h + "</td><td>" + s + "</td></tr>");
                }
                sb.Append("</tbody></table>");
            }
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString())))
            {
                ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(byteArrayOutputStream);
                PdfDocument pdfDocument = new PdfDocument(writer);
                pdfDocument.SetDefaultPageSize(PageSize.A4);
                HtmlConverter.ConvertToPdf(stream, pdfDocument);
                pdfDocument.Close();
               return  File(byteArrayOutputStream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }
        [HttpPost]
        public ActionResult ExportEmployeeReport(string employee, string year)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<h3>" + year + "Year Report (" + employee + ")</h3><table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #d9d9d9; font-size: 6pt; margin-bottom: 20px;'><thead><tr><th style='background-color: #bdd7ee;'>Division</th>");
            var data = db.Histories.Where(e => e.Employee == employee && e.Date.Contains(year)).ToList();
            int[] months = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var checkYear = DateTime.Now.Year;
            if (checkYear % 4 == 0 && (checkYear % 100 != 0 || checkYear % 400 == 0))
            {
                months[2] = 29;
            }

            for (var j = 1; j <= 31; j++) 
            {
                sb.Append("<th style='background-color: #ffe699;'>" + j + "</th>");
            }
            sb.Append("</tr></thead><tbody>");
            for (var i = 1; i <= 12; i++)
            {
                sb.Append("<tr><td>" + i + "</td>");
                for (var k = 1; k <= months[i]; k++)
                {
                    var state = false;
                    var info = "";
                    var time = "";
                    if(k < 10)
                    {
                        time = "0" + k;
                    } else
                    {
                        time = k.ToString();
                    }
                    if(i < 10)
                    {
                        time += ".0" + i;
                    } else
                    {
                        time += "." + i;
                    }
                    time += "." + year;
                    var date = new DateTime(Convert.ToInt32(year), Convert.ToInt32(i), Convert.ToInt32(k));
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        sb.Append("<td style='background-color: #ededed;'></td>");
                    }
                    else
                    {
                        foreach(var item in data)
                        {
                            if(item.Date.ToString() == time.ToString())
                            {
                                state = true;
                                info = item.State;
                            }
                        }
                        if (state == true)
                        {
                            if (info == "H")
                            {
                                sb.Append("<td style='background-color: #c6e0b4;'>" + info + "</td>");
                            }
                            else
                            {
                                sb.Append("<td style='background-color: #fce4d6;'>" + info + "</td>");
                            }
                        }
                        else
                        {
                            sb.Append("<td></td>");
                        }
                    }
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString())))
            {
                ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(byteArrayOutputStream);
                PdfDocument pdfDocument = new PdfDocument(writer);
                pdfDocument.SetDefaultPageSize(PageSize.A4);
                HtmlConverter.ConvertToPdf(stream, pdfDocument);
                pdfDocument.Close();
                return File(byteArrayOutputStream.ToArray(), "application/pdf", "Grid.pdf");
            }
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}