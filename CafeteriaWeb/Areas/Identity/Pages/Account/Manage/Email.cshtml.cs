// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CafeteriaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CafeteriaWeb.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Novo Email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
               string htmlEmailMessage = $"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"border-collapse: collapse;\">\r\n" +
                        "        <tr>\r\n" +
                        "            <td align=\"center\" bgcolor=\"#ffffff\" style=\"padding: 40px 0 30px 0;\">\r\n" +
                        "                <img src=\"https://example.com/logo.png\" alt=\"Logo\" width=\"150\">\r\n" +
                        "                <h1 style=\"color: #333;\">Confirme seu endereço de email</h1>\r\n" +
                        "                <p style=\"color: #777;\">Por favor, confirme sua conta clicando no botão abaixo:</p>\r\n" +
                        $"                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"display: inline-block; background-color: #007bff; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px;\">" +
                        "                   Confirmar Email</a>\r\n" +
                        "                <p style=\"color: #777; margin-top: 20px;\">Se você não se registrou em nosso site, ignore este email.</p>\r\n" +
                        "            </td>\r\n" +
                        "        </tr>\r\n" +
                        "        <tr>\r\n" +
                        "            <td bgcolor=\"#f4f4f4\" style=\"text-align: center; padding: 10px;\">\r\n" +
                        $"                &copy; {DateTime.Now.Year} CafeteriaWeb. Todos os direitos reservados.\r\n" +
                        "            </td>\r\n" +
                        "        </tr>\r\n" +
                        "    </table>";


                    await _emailSender.SendEmailAsync(Input.NewEmail, "Confirme seu email", htmlEmailMessage);

                StatusMessage = "Link de confirmação para alterar o email enviado. Por favor, verifique o seu email.";
                return RedirectToPage();
            }

            StatusMessage = "Seu email não foi alterado.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            string htmlEmailMessage = $"<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"border-collapse: collapse;\">\r\n" +
                        "        <tr>\r\n" +
                        "            <td align=\"center\" bgcolor=\"#ffffff\" style=\"padding: 40px 0 30px 0;\">\r\n" +
                        "                <img src=\"https://example.com/logo.png\" alt=\"Logo\" width=\"150\">\r\n" +
                        "                <h1 style=\"color: #333;\">Confirme seu endereço de email</h1>\r\n" +
                        "                <p style=\"color: #777;\">Por favor, confirme sua conta clicando no botão abaixo:</p>\r\n" +
                        $"                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"display: inline-block; background-color: #007bff; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px;\">" +
                        "                   Confirmar Email</a>\r\n" +
                        "                <p style=\"color: #777; margin-top: 20px;\">Se você não se registrou em nosso site, ignore este email.</p>\r\n" +
                        "            </td>\r\n" +
                        "        </tr>\r\n" +
                        "        <tr>\r\n" +
                        "            <td bgcolor=\"#f4f4f4\" style=\"text-align: center; padding: 10px;\">\r\n" +
                        $"                &copy; {DateTime.Now.Year} CafeteriaWeb. Todos os direitos reservados.\r\n" +
                        "            </td>\r\n" +
                        "        </tr>\r\n" +
                        "    </table>";

            await _emailSender.SendEmailAsync(
                email,
                "Confirme seu email",
                htmlEmailMessage);

            StatusMessage = "Link de confirmação para alterar o email enviado. Por favor, verifique o seu email.";
            return RedirectToPage();
        }
    }
}
