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
using Microsoft.AspNetCore.Identity;

namespace CafeteriaWeb.Controllers
{
    public class AdressesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly AdressService _adressService;

        public AdressesController(ApplicationDbContext context, 
            UserService userService,
            AdressService adressService, 
            UserManager<User> userManager)
        {
            _context = context;
            _userService = userService;
            _adressService = adressService;
            _userManager = userManager;
        }

        // GET: Adresses
        public async Task<IActionResult> Index()
        {
              return _context.Adress != null ? 
                          View(await _context.Adress.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Adress'  is null.");
        }


        // GET: Adresses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,Neighborhood,Number,Cep,Town,Complement")] Adress adress)
        {
            if (ModelState.IsValid)
            {
                List<Adress> adresses = new();
                User user = _userService.FindByUserName(User.Identity.Name);                
                if (adress.Name == null)
                {
                    adresses = _adressService.ListByUserId(user.Id);
                    adress.Name = "Endereço " + (adresses.Count+1).ToString();
                }
                await _adressService.InsertAsync(adress);

                adresses.Add(adress);

                user.Adresses = adresses;
                await _userService.UpdateAsync(user);

                TempData["Message"] = "Endereço incluido com Sucesso!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });

                //return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = "Erro - Dados Invalidos!";
            //return View(adress);
            return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
        }

        // GET: Adresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Adress == null)
            {
                TempData["Message"] = "Erro - Dados não encontrados!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }

            var adress = await _adressService.FindByIdAsync(id.Value);
            if (adress == null)
            {
                TempData["Message"] = "Erro - Dados não encontrados!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }
            return View(adress);
        }

        // POST: Adresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,Neighborhood,Number,Cep,Town,Complement")] Adress adress)
        {
            if (id != adress.Id)
            {
                TempData["Message"] = "Erro - Dados não encontrados!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }

            if (ModelState.IsValid)
            {
                try
                {                    
                    await _adressService.UpdateAsync(adress);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdressExists(adress.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Endereço editado com Sucesso!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }
            TempData["Message"] = "Erro - Dados não encontrados!";
            return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
        }

        // GET: Adresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Adress == null)
            {
                TempData["Message"] = "Erro - Dados não encontrados!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }

            var adress = await _adressService.FindByIdAsync(id.Value);
            if (adress == null)
            {
                TempData["Message"] = "Erro - Dados não encontrados!";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }

            return View(adress);
        }

        // POST: Adresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Adress == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Adress'  is null.");
            }
            var adress = await _adressService.FindByIdAsync(id);
            if (adress != null)
            {
               await _adressService.RemoveAsync(id);
            }
            
            //await _context.SaveChangesAsync();
            TempData["Message"] = "Endereço Removido com Sucesso!";
            return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            
        }

        private bool AdressExists(int id)
        {
          return (_context.Adress?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
