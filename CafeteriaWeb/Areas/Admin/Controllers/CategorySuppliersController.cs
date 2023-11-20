using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using CafeteriaWeb.Services;
using Newtonsoft.Json;

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class CategorySuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CategorySupplierService _categorySupplierService;

        public CategorySuppliersController(ApplicationDbContext context,
            CategorySupplierService categorySupplierService)
        {
            _context = context;
            _categorySupplierService = categorySupplierService;
        }

        // GET: Admin/CategorySuppliers
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
            return View(await _categorySupplierService.ListAllAsync());
        }


        // GET: Admin/CategorySuppliers/Create
        public IActionResult Create()
        {
            CategorySupplier categorySupplier = new();
            if (TempData["CategoryCreate"] != null)
                categorySupplier = JsonConvert.DeserializeObject<CategorySupplier>(TempData["CategoryCreate"].ToString());
            //category.StatusMessage = (string)ViewBag.StatusMessage;
            return View(categorySupplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] CategorySupplier categorySupplier)
        {
            if (ModelState.IsValid)
            {
                categorySupplier.ModifyedOn = DateTime.Now;
                await _categorySupplierService.InsertAsync(categorySupplier);
                return RedirectToAction(nameof(Index));
            }
            categorySupplier.StatusMessage = "Erro! - Houve um erro ao realizar o cadastro, por favor verifique os dados e tente novamente.";
            TempData["CategoryCreate"] = JsonConvert.SerializeObject(categorySupplier);
            TempData["HasMessage"] = true;
            return RedirectToAction(nameof(Index));
            //return View(category);
        }

        // GET: Admin/CategorySuppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorySupplier = await _categorySupplierService.FindByIdAsync(id.Value);
            if (categorySupplier == null)
            {
                return NotFound();
            }
            return View(categorySupplier);
        }

        // POST: Admin/CategorySuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Enabled,CreatedOn")] CategorySupplier categorySupplier)
        {
            if (id != categorySupplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    categorySupplier.ModifyedOn = DateTime.Now;
                    await _categorySupplierService.UpdateAsync(categorySupplier);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_categorySupplierService.CategoryExists(categorySupplier.Id))
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
            return View(categorySupplier);
        }

        // GET: Admin/CategorySuppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorySupplier = await _categorySupplierService.FindByIdAsync(id.Value);
            if (categorySupplier == null)
            {
                return NotFound();
            }

            return View(categorySupplier);
        }

        // POST: Admin/CategorySuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_categorySupplierService == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CategorySupplier'  is null.");
            }
            await _categorySupplierService.RemoveAsync(id);            
            return RedirectToAction(nameof(Index));
        }
    }
}
