using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CafeteriaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using CafeteriaWeb.Services;
using System.Globalization;

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;
        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
              return View(await _employeeService.ListAllEnabledAsync());
        }

        public async Task<IActionResult> FilterEmployees(bool allEmploeeys)
        {
            ViewBag.EmploeeysFilter = allEmploeeys;
            return View("Index",allEmploeeys ? await _employeeService.ListAllAsync() : await _employeeService.ListAllEnabledAsync());
        }

        // GET: Admin/Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Wage")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                string wageText = employee.Wage.ToString();
                decimal wage = 0;
                if (wageText.Length >= 3)
                {
                    string decimalPart = wageText.Substring(wageText.Length - 2);
                    wageText = wageText.Substring(0, wageText.Length - 2) + "." + decimalPart;
                    wage = decimal.Parse(wageText, CultureInfo.InvariantCulture);
                }
                employee.Wage = wage;
                await _employeeService.InsertAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.FindByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Wage,Enabled,CreatedOn")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wageText = employee.Wage.ToString();
                    decimal wage = 0;
                    if (wageText.Length >= 3)
                    {
                        string decimalPart = wageText.Substring(wageText.Length - 2);
                        wageText = wageText.Substring(0, wageText.Length - 2) + "." + decimalPart;
                        wage = decimal.Parse(wageText, CultureInfo.InvariantCulture);
                    }
                    employee.Wage = wage;
                    await _employeeService.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Admin/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.FindByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
