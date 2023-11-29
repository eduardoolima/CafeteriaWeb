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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CafeteriaWeb.Models.Enums;

namespace CafeteriaWeb.Controllers
{
    [Authorize]
    public class FeedbacksController : Controller
    {
        private readonly FeedbackService _feedbackService;
        private readonly NotificationService _notificationService;
        private readonly OrderService _orderService;
        private readonly UserManager<User> _userManager;
        public FeedbacksController(FeedbackService feedbackService,
            NotificationService notificationService,
            OrderService orderService,
            UserManager<User> userManager)
        {
            _feedbackService = feedbackService;
            _notificationService = notificationService;
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            return View(await _feedbackService.ListAllAsync());
        }


        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var feedback = await _feedbackService.FindByIdAsync(id.Value);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text")] Feedback feedback)
        {
            try
            {
                User user = await _userManager.GetUserAsync(User);                              
                feedback.User = user;
                feedback.UserId = user.Id;
                await _feedbackService.InsertAsync(feedback);
                TempData["Message"] = null;

                User admin = _userManager.FindByNameAsync("gerente@caf");
                Notification notification = new()
                {
                    Title = "Novo Feedback",
                    Text = $"O cliente {user.FirstName + user.LastName} compartilhou sua experiência na cafeteria conosco. Confira o feedback!",
                    UserToNotify = admin,
                    UserToNotifyId = admin.Id,
                    NotificationType = NotificationType.NewFeedback
                };
                _notificationService.CreateNotification(notification);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Message"] = "Houve um erro ao Registrar seu feedback, por favor, tente novamente mais tarde.";
                return RedirectToAction(nameof(Index));
            }          
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _feedbackService.FindByIdAsync(id.Value);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _feedbackService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
