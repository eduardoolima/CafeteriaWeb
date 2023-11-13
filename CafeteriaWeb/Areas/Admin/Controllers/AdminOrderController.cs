using CafeteriaWeb.Models;
using CafeteriaWeb.Models.Enums;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        private readonly UserManager<User> _userManager;
        private PaymentConfiguration _paymentConfiguration;
        private NotificationConfiguration _notificationConfiguration;
        public AdminOrderController(OrderService orderService,
            ShoppingCart shoppingCart,
            AdressService adressService,
            NotificationService notificationService,
            UserManager<User> userManager,
            IOptions<PaymentConfiguration> paymentConfiguration,
            NotificationConfiguration notificationConfiguration)
        {
            _orderService = orderService;
            _shoppingCart = shoppingCart;
            _adressService = adressService;
            _notificationService = notificationService;
            _userManager = userManager;
            _paymentConfiguration = paymentConfiguration.Value;
            _notificationConfiguration = notificationConfiguration;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderService.ListAll();
            return View(orders);
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
