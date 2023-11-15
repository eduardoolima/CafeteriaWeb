using CafeteriaWeb.Models;
using CafeteriaWeb.Models.Enums;
using CafeteriaWeb.Services;
using CafeteriaWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminOrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ShoppingCart _shoppingCart;
        private readonly AdressService _adressService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly UserManager<User> _userManager;
        private NotificationConfiguration _notificationConfiguration;
        public AdminOrderController(OrderService orderService,
            ShoppingCart shoppingCart,
            AdressService adressService,
            NotificationService notificationService,
            UserService userService,
            ProductService productService,
            UserManager<User> userManager,
            IOptions<NotificationConfiguration> notificationConfiguration)
        {
            _orderService = orderService;
            _shoppingCart = shoppingCart;
            _adressService = adressService;
            _notificationService = notificationService;
            _userService = userService;
            _productService = productService;
            _userManager = userManager;
            _notificationConfiguration = notificationConfiguration.Value;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderService.ListAll();
            return View(orders);
        }

        public async Task<IActionResult> PeriodFilter(string period)
        {
            ViewBag.Period = period;
            switch (period)
            {
                case "day":
                    {
                        var orders = await _orderService.ListAllByDayAsync(DateTime.Today);
                        return View("Index", orders);
                    }
                case "week":
                    {
                        var orders = await _orderService.ListAllByPeriodAsync(GetFirstDayOfWeek(), DateTime.Today);
                        return View("Index", orders);
                    }
                case "month":
                    {
                        var orders = await _orderService.ListAllByPeriodAsync(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Today);
                        return View("Index", orders);
                    }
                case "year":
                    {
                        var orders = await _orderService.ListAllByPeriodAsync(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Today);
                        return View("Index", orders);
                    }
                default:
                    {
                        return RedirectToAction("Index");
                    }
            }
        }

        static DateTime GetFirstDayOfWeek()
        {
            DayOfWeek firstDay = DayOfWeek.Sunday;

            int difference = DateTime.Now.DayOfWeek - firstDay;

            if (difference < 0)
                difference += 7;
            return DateTime.Now.AddDays(-difference).Date;
        }

        public async Task<IActionResult> Create()
        {
            LoadUsers();
            var products = await _productService.ListAllAsync();
            ViewBag.Products = products;
            return View();
        }
        [HttpPost]
        //public async Task<IActionResult> CreateOrder([FromBody] AdminOrderViewModel orderData)
        public IActionResult CreateOrder(string jsonData)        
        {
            AdminOrderViewModel orderViewModel = JsonConvert.DeserializeObject<AdminOrderViewModel>(jsonData);
            int totalItensOrder = 0;
            decimal totalPriceOrder = 0.0m;
            foreach (var item in orderViewModel.Products)
            {
                decimal price = 0;
                string priceText = item.Price.ToString();
                if (priceText.Length >= 3)
                {
                    string decimalPart = priceText.Substring(priceText.Length - 2);
                    priceText = priceText.Substring(0, priceText.Length - 2) + "." + decimalPart;
                    price = decimal.Parse(priceText, CultureInfo.InvariantCulture);
                }
                item.Price = price;
                if(item.Amount>0)
                    totalPriceOrder += item.Price * item.Amount;
            }
            List<Products> orderProducts = orderViewModel.Products.Where(p => p.Amount>0).ToList();
            Order order = new();
            order.IsPaid = orderViewModel.Order.IsPaid;
            order.ForDelivery = orderViewModel.Order.ForDelivery;
            order.TotalItensOrder = totalItensOrder;
            order.TotalOrder = totalPriceOrder;

            if(orderViewModel.Order.UserId != null && orderViewModel.Order.UserId != "0" )
            {
                var user = _userService.FindById(orderViewModel.Order.UserId);
                order.User = user;
                order.UserId = user.Id;
            }
            else
            {
                var user = _userService.FindByUserName("ClienteExterno");
                order.User = user;
                order.UserId = user.Id;
            }
            if(orderViewModel.Order.AdressId != null && orderViewModel.Order.AdressId != 0)
            {
                var adress = _adressService.FindById(orderViewModel.Order.AdressId.Value);
                order.Adress = adress;
                order.AdressId = adress.Id;
            }
            _orderService.CreateOrderAdmin(order, orderProducts);
            return Ok();
        }

        void LoadUsers()
        {
            var users = _userService.FindAll();
            foreach (var item in users)
            {
                item.CompleteName = item.FirstName + " " + item.LastName;
            }
            users.Insert(0, new User { Id = "0", CompleteName = "Selecione" });
            ViewBag.Users = new SelectList(users, "Id", "CompleteName");
        }

        public IActionResult ListAdress(string userId)
        {
            var adresses = _adressService.ListByUserId(userId);
            adresses.Insert(0, new Adress { Id = 0, Name = "Selecione", Street = "Selecione", Number = "" });
            return Json(adresses);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderService.FindById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Edit(int id, [Bind("Id, Observation, Adress")] Order orderEdit)
        {
            if (id != orderEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            orderEdit.StatusMessage = "Erro - Houve um erro ao editar, por favor, verifique as informações e tente novamente.";
            return View(orderEdit);
        }

        public async Task<IActionResult> MarkOrderAsPayed(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }
            var order = _orderService.FindById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpPost, ActionName("MarkOrderAsPayed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkOrderAsPayedConfirmed(int id)
        {
            await _orderService.MarkOrderAsPayedAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> FinishOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderService.FindById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpPost, ActionName("FinishOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishOrderConfirmed(int id)
        {
            await _orderService.FinishOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OutForDelivery(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderService.FindById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [HttpPost, ActionName("OutForDelivery")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OutForDeliveryConfirmed(int id)
        {
            await _orderService.OutForDeliveryAsync(id);
            var order = await _orderService.FindByIdAsync(id);

            Notification notification = new()
            {
                NotificationType = NotificationType.OutForDelivery,
                UserToNotify = order.User,
                UserToNotifyId = order.User.Id,
                Title = _notificationConfiguration.NotificationOutForDeliveryTitle,
                Text = _notificationConfiguration.NotificationOutForDeliveryText
            };

            _notificationService.CreateNotification(notification);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderService.FindById(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
