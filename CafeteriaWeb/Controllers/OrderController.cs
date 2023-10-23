using CafeteriaWeb.Controllers;
using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using CafeteriaWeb.ViewModel;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Web;

namespace LanchesMac.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ShoppingCart _shoppingCart;
        private readonly AdressService _adressService;
        private readonly UserManager<User> _userManager;
        private PaymentConfiguration _paymentConfiguration;
        public OrderController(OrderService orderService,
            ShoppingCart shoppingCart,
            AdressService adressService,
            UserManager<User> userManager,
            IOptions<PaymentConfiguration> paymentConfiguration)                    
        {
            _orderService = orderService;
            _shoppingCart = shoppingCart;
            _adressService = adressService;
            _userManager = userManager;
            _paymentConfiguration = paymentConfiguration.Value;
        }

        [Authorize]
        public IActionResult FinishOrder()
        {
            ListAdress();
            return View();
        }

        [Authorize]
        [HttpPost]        
        public async Task<IActionResult> FinishOrder(Order order)
        {
            string currentUrl = Request.Headers["Referer"].ToString();
            string[] splitUrl = currentUrl.Split("Order");
            string notificationUrl = splitUrl[0] + "Order/MercadoPagoWebhook";

            MercadoPagoConfig.AccessToken = _paymentConfiguration.MercadoPagoAccessToken;
            string copyPaste = string.Empty;
            string qrCode = string.Empty;
            string url = string.Empty;
            int totalItensOrder = 0;
            decimal totalPriceOrder = 0.0m;
            
            List<ShoppingCartItem> items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if(_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Seu carrinho está vazio!");
                return View(order);
            }
            foreach(var item in items)
            {
                totalItensOrder += item.Amount;
                totalPriceOrder += item.Product.Price * item.Amount;
            }
            var user = await _userManager.GetUserAsync(User);
            order.TotalItensOrder = totalItensOrder;
            order.TotalOrder = totalPriceOrder;
            order.User = user;
            order.UserId = user.Id;
            order.Adress = _adressService.FindById(order.AdressId);            
            if (order.PaymentOnline && order.PaymentMethod == CafeteriaWeb.Models.Enums.PaymentMethod.Pix)
            {
                var request = new PaymentCreateRequest
                {
                    TransactionAmount = totalPriceOrder,
                    Description = _paymentConfiguration.PixDescription,
                    PaymentMethodId = "pix",
                    //NotificationUrl = "https://46c5-187-86-96-94.ngrok-free.app/webhook/receber-notificacoes",
                    NotificationUrl = notificationUrl,
                    Payer = new PaymentPayerRequest
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName                     
                    },
                };

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(request);

                copyPaste = payment.PointOfInteraction.TransactionData.QrCode;
                qrCode = payment.PointOfInteraction.TransactionData.QrCodeBase64;
                url = payment.PointOfInteraction.TransactionData.TicketUrl;
                order.TransactionId = payment.Id.ToString();
            }            
            OrderViewModel orderViewModel = new()
            {
                Order = order,
                PixCopyPaste = copyPaste,
                PixQrCode = qrCode,
            };
            //if (ModelState.IsValid)
            //{
            _orderService.CreateOrder(order);
            ViewBag.CheckoutMessage = "Obrigado pelo seu pedido :)";
            ViewBag.TotalOrder = _shoppingCart.GetShoppingCartTotal();

            _shoppingCart.ClearShoppingCart();
            return View("~/Views/Order/CheckoutComplete.cshtml", orderViewModel);
            //}
            //ListAdress();
            //return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> MercadoPagoWebhook()
        {
            using (StreamReader reader = new(Request.Body))
            {
                string requestBody = await reader.ReadToEndAsync();
                dynamic? jsonData = JsonConvert.DeserializeObject(requestBody);
                if (jsonData != null)
                {
                    if (jsonData.action == "payment.updated")
                    {                       
                        string paymentid = jsonData.data.id;
                        await GetPaymentStatus(paymentid);
                    }
                }              
            }
            // Responder com um status HTTP 200 OK para confirmar o recebimento da notificação
            return Ok();
        }

        public async Task<IActionResult> GetPaymentStatus(string paymentId)
        {
            MercadoPagoConfig.AccessToken = _paymentConfiguration.MercadoPagoAccessToken;
            string apiUrl = "https://api.mercadopago.com/v1/payments/{id}";
            string accessToken = MercadoPagoConfig.AccessToken;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                string transactionId = paymentId;

                string fullUrl = apiUrl.Replace("{id}", transactionId);

                HttpResponseMessage response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic? jsonData = JsonConvert.DeserializeObject(responseBody);
                    if (jsonData.status == "approved")
                    {
                        Order order = await _orderService.FindByTransactionIdAsync(paymentId);
                        order.IsPaid = true;
                        await _orderService.UpdateAsync(order);
                        Console.WriteLine("Pagamento " + jsonData.status);
                    }
                }
                else
                {
                    Console.WriteLine("Erro na solicitação. Status Code: " + response.StatusCode);
                }
            }
            return Ok();
        }

        void ListAdress()
        {
            var userId = _userManager.GetUserId(User);
            var categories = _adressService.ListByUserId(userId);
            categories.Insert(0, new Adress { Id = 0, Name = "Selecione" });
            ViewBag.Adresses = new SelectList(categories, "Id", "Name");
        }
    }
}
