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

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SupplierService _supplierService;
        private readonly CategorySupplierService _categorySupplierService;

        public SuppliersController(ApplicationDbContext context,
            SupplierService supplierService,
            CategorySupplierService categorySupplierService)
        {
            _context = context;
            _supplierService = supplierService;
            _categorySupplierService = categorySupplierService;
        }

        void ListCategories()
        {
            var categories = _categorySupplierService.ListAll();
            categories.Insert(0, new CategorySupplier { Id = 0, Name = "Selecione" });
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
        }

        // GET: Admin/Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await _supplierService.ListAllAsync());
                         
        }

        // GET: Admin/Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _supplierService.FindByIdAsync(id.Value);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Admin/Suppliers/Create
        public IActionResult Create()
        {
            ListCategories();
            return View();
        }

        // POST: Admin/Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Adress,PhoneNumber,Email,Description,CategorySupplierId")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.InsertAsync(supplier);
                return RedirectToAction(nameof(Index));
            }
            ListCategories();
            return View(supplier);
        }

        // GET: Admin/Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _supplierService.FindByIdAsync(id.Value);
            if (supplier == null)
            {
                return NotFound();
            }
            ListCategories();
            return View(supplier);
        }

        // POST: Admin/Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Adress,PhoneNumber,Email,Description,CategorySupplierId,Enabled,CreatedOn")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierService.UpdateAsync(supplier);                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_supplierService.SupplierExists(supplier.Id))
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
            ListCategories();
            return View(supplier);
        }

        // GET: Admin/Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _supplierService.FindByIdAsync(id.Value);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Admin/Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Suppliers'  is null.");
            }
            await _supplierService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }        
    }
}
