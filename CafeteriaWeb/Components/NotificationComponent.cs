using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using CafeteriaWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaWeb.Components
{
    public class NotificationComponent : ViewComponent
    {
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;

        public NotificationComponent(NotificationService notificationService,
            UserService userService,
            UserManager<User> userManager)
        {
            _notificationService = notificationService;
            _userService = userService;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);           

            var notificationVm = new NotificationViewModel()
            {
                User = _userService.FindById(userId)
            };
            return View(notificationVm);
        }
    }
}
