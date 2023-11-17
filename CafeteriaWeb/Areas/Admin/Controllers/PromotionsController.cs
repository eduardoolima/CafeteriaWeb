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
using CafeteriaWeb.ViewModel;
using Newtonsoft.Json;

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;
        private readonly PromotionService _promotionService;
        public PromotionsController(ApplicationDbContext context, 
            PromotionService promotionService,
            ProductService productService)
        {
            _context = context;
            _promotionService = promotionService;
            _productService = productService;
        }

        // GET: Admin/Promotions
        public async Task<IActionResult> Index()
        {
            var promotions = await _promotionService.ListAllAsync();
            return View(promotions);                          
        }

        // GET: Admin/Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _promotionService.FindByIdAsync(id.Value);
                
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Admin/promotions/Create

        public async Task<IActionResult> Create()
        {
            var products = await _productService.ListAllAsync();
            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public IActionResult CreatePromotion(string jsonData)
        {
            PromotionViewModel promotionViewModel = JsonConvert.DeserializeObject<PromotionViewModel>(jsonData);
            Promotion promotion = new();
            List<Product> products = new();
            foreach(var item in promotionViewModel.Products)
            {
                Product product = _productService.FindById(item);
                products.Add(product);
            }
            promotion.Products = products;
            promotion.OnSalePrice = promotionViewModel.OnSalePrice;
            promotion.SaleStart = promotionViewModel.SaleStart;
            promotion.SaleEnd = promotionViewModel.SaleEnd;
            _promotionService.Insert(promotion);
            return Ok();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _productService.ListAllAsync();
            ViewBag.Products = products;

            var promotion = await _promotionService.FindByIdAsync(id.Value);
            List<int> productIds = new();
            foreach(var item in promotion.Products)
            {
                productIds.Add(item.Id);
            }
            PromotionViewModel promotionViewModel = new()
            {
                PromotionId = promotion.Id,
                Products = productIds,
                OnSalePrice = promotion.OnSalePrice,
                SaleStart = promotion.SaleStart,
                SaleEnd = promotion.SaleEnd
            };
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotionViewModel);
        }

        [HttpPost]
        public IActionResult EditPromotion(string jsonData)
        {
            PromotionViewModel promotionViewModel = JsonConvert.DeserializeObject<PromotionViewModel>(jsonData);            
            Promotion promotion = _promotionService.FindById(promotionViewModel.PromotionId.Value);
            if(promotion != null)
            {
                List<Product> products = new();
                foreach (var item in promotionViewModel.Products)
                {
                    Product product = _productService.FindById(item);
                    products.Add(product);
                }
                promotion.Products = products;
                promotion.OnSalePrice = promotionViewModel.OnSalePrice;
                promotion.SaleStart = promotionViewModel.SaleStart;
                promotion.SaleEnd = promotionViewModel.SaleEnd;
                _promotionService.Update(promotion);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _promotionService.FindByIdAsync(id.Value);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _promotionService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
