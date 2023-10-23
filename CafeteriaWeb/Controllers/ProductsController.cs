using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp.Formats.Png;

namespace CafeteriaWeb.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productsService;
        private readonly CategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, 
            ProductService productService, CategoryService categoryService,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _productsService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ListCategories();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,SmallDescription,Description,ImgPath,IsAvaible,CategoryId")] Product product, IFormFile img)
         {
            try
            {
                if (ModelState.IsValid)
                {
                    string priceText = product.Price.ToString();
                    if (priceText.Length >= 3)
                    {
                        string decimalPart = priceText.Substring(priceText.Length - 2);
                        priceText = priceText.Substring(0, priceText.Length - 2) + "." + decimalPart;
                        product.Price = decimal.Parse(priceText, CultureInfo.InvariantCulture);
                    }
                    product.Enabled = true;
                    product.CreatedOn = DateTime.Now;
                    product.ModifyedOn = DateTime.Now;
                    product.IsFavorite = false;
                    product.Category = await _categoryService.FindByIdAsync(product.CategoryId);
                    if (img != null)
                    {
                        string pathImg = string.Empty;
                        string pathImgThumbnail = string.Empty;
                        DateTime getNow = DateTime.Now;
                        string now = getNow.ToString().Replace(" ", "").Replace("/", "").Replace(":", "");
                        string pathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Img/ProductImg");
                        string pathFileThumbnail = Path.Combine(_webHostEnvironment.WebRootPath, "Img/ProductThumbnail");
                        string fileName = $" {now}-Product-{product.Name}-{product.Id}.png";
                        string fileNameThumbnail = "Thumbnail-" + fileName;
                        using (FileStream fileStream = new(Path.Combine(pathFile, fileName), FileMode.Create))
                        {
                            await img.CopyToAsync(fileStream);
                            pathImg = "~/Img/ProductImg/" + fileName;
                        }
                        product.ImgPath = pathImg;
                        var image = Image.Load(img.OpenReadStream());
                        using (var outputStream = new MemoryStream())
                        {
                            int width = 60;
                            int heigth = 40;// (int)((width / (double)image.Width) * image.Height);
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(width, heigth),
                                Mode = ResizeMode.Max
                            }));
                            image.Save(outputStream, new PngEncoder());
                            var imageStream = new MemoryStream(outputStream.ToArray());
                            IFormFile imgThumbnail = new FormFile(imageStream, 0, imageStream.Length, "formFile", img.FileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = img.ContentType
                            };
                            using (FileStream fileStream = new(Path.Combine(pathFileThumbnail, fileNameThumbnail), FileMode.Create))
                            {
                                await imgThumbnail.CopyToAsync(fileStream);
                            }
                        }
                        pathImgThumbnail = "~/Img/ProductThumbnail/" + fileNameThumbnail;
                        product.ImgThumbnailPath = pathImgThumbnail;
                    }
                    await _productsService.InsertAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                ListCategories();
                product.StatusMessage = "Erro - Houve um erro durante o cadastro, por favor, verifique as informações e tente novamente.";
                return View(product);
            }
            catch (Exception)
            {
                product.StatusMessage = "Erro fatal! entre em contato com o administrador do sistema";
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            
            var product = await _productsService.FindByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }            
            ListCategories();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,SmallDescription,Description,IsFavorite,IsAvaible,CategoryId,ImgPathOld,ImgThumbnailPathOld")] Product productEdit, IFormFile? img)
        {
            if (id != productEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Product product = await _productsService.FindByIdAsync(id);
                    string priceText = productEdit.Price.ToString();
                    if(priceText.Length >= 3)
                    {
                        string decimalPart = priceText.Substring(priceText.Length - 2);
                        priceText = priceText.Substring(0, priceText.Length - 2) + "." + decimalPart;
                        product.Price = decimal.Parse(priceText, CultureInfo.InvariantCulture);
                    }                    
                    product.Name = productEdit.Name;
                    product.IsAvaible = productEdit.IsAvaible;
                    product.Description = productEdit.Description;
                    product.SmallDescription = productEdit.SmallDescription;                    
                    product.Category = await _categoryService.FindByIdAsync(productEdit.CategoryId);
                    product.ModifyedOn = DateTime.Now;

                    if (img != null)
                    {
                        string pathImg = string.Empty;
                        string pathImgThumbnail = string.Empty;
                        DateTime getNow = DateTime.Now;
                        string now = getNow.ToString().Replace(" ", "").Replace("/", "").Replace(":", "");
                        string pathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Img/ProductImg");
                        string pathFileThumbnail = Path.Combine(_webHostEnvironment.WebRootPath, "Img/ProductThumbnail");
                        string fileName = $" {now}-Product-{product.Name}-{product.Id}.png";
                        string fileNameThumbnail = "Thumbnail-" + fileName;
                        using (FileStream fileStream = new(Path.Combine(pathFile, fileName), FileMode.Create))
                        {
                            await img.CopyToAsync(fileStream);
                            pathImg = "~/Img/ProductImg/" + fileName;
                        }
                        product.ImgPath = pathImg;
                        var image = Image.Load(img.OpenReadStream());
                        using (var outputStream = new MemoryStream())
                        {
                            int width = 60;
                            int heigth = 40;// (int)((width / (double)image.Width) * image.Height);
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(width, heigth),
                                Mode = ResizeMode.Max
                            }));
                            image.Save(outputStream, new PngEncoder());
                            var imageStream = new MemoryStream(outputStream.ToArray());
                            IFormFile imgThumbnail = new FormFile(imageStream, 0, imageStream.Length, "formFile", img.FileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = img.ContentType
                            };
                            using (FileStream fileStream = new(Path.Combine(pathFileThumbnail, fileNameThumbnail), FileMode.Create))
                            {
                                await imgThumbnail.CopyToAsync(fileStream);
                            }
                        }
                        pathImgThumbnail = "~/Img/ProductThumbnail/" + fileNameThumbnail;
                        product.ImgThumbnailPath = pathImgThumbnail;
                    }

                    await _productsService.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    productEdit.StatusMessage = "Erro fatal! entre em contato com o administrador do sistema";
                    if (!ProductExists(productEdit.Id))
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
            productEdit.StatusMessage = "Erro - Houve um erro ao editar, por favor, verifique as informações e tente novamente.";
            ListCategories();
            return View(productEdit);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        void ListCategories()
        {
            var categories = _categoryService.ListAll();
            categories.Insert(0, new Category { Id = 0, Name = "Selecione" });
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
        }
    }
}
