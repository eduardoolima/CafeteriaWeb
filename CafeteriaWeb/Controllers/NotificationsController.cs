using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public NotificationsController(UserService userService,
            NotificationService notificationService,
            UserManager<User> userManager)
        {
            _userService = userService;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var notifications = await _notificationService.ListByUserAsync(userId);
                return View(notifications);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _notificationService.FindByIdAsync(id.Value);
            if (notification == null)
            {
                return NotFound();
            }
            await _notificationService.ReadNotificationAsync(notification);
            return View(notification);
        }
    }
}
