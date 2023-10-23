using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace CafeteriaWeb.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;        

        public CategoriesController(ApplicationDbContext context, UserService userService, CategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
            _userService = userService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            if (TempData["HasMessage"] != null && (bool)TempData["HasMessage"])
            {
                ViewBag.HasMessage = true;               
            }
            else
            {
                ViewBag.HasMessage = false;
            }
            return _context.Categories != null ? 
                          View(await _categoryService.ListAllAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }
       

        // GET: Categories/Create
        public IActionResult Create()
        {
            Category category = new();
            if(TempData["CategoryCreate"] != null)
                category = JsonConvert.DeserializeObject<Category>(TempData["CategoryCreate"].ToString());
            //category.StatusMessage = (string)ViewBag.StatusMessage;
            return View(category);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedOn = DateTime.Now;
                category.ModifyedOn = DateTime.Now;
                category.Enabled = true;
                await _categoryService.InsertAsync(category);
                return RedirectToAction(nameof(Index));
            }
            category.StatusMessage = "Erro! - Houve um erro ao realizar o cadastro, por favor tente novamente.";
            TempData["CategoryCreate"] = JsonConvert.SerializeObject(category);
            TempData["HasMessage"] = true;            
            return RedirectToAction(nameof(Index));
            //return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryService.FindByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.ModifyedOn = DateTime.Now;
                    await _categoryService.UpdateAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryService.FindByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _categoryService.FindByIdAsync(id);
            category.ModifyedOn = DateTime.Now;
            category.Enabled = false;
            await _categoryService.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
