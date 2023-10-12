// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeteriaWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AdressService _adressService;
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AdressService adressService,
            UserService userService,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _adressService = adressService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
        }

        public User InputUser { get; set; }
        public Adress Adress { get; set; }
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }


        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var adresses = _adressService.ListByUserId(user.Id);
            InputUser = user;
            InputUser.Adresses = adresses;
            Username = userName;
            //InputUser.PhoneNumber = phoneNumber;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            if(TempData["Message"] != null)
            {
                var message = TempData["Message"] as string;
                StatusMessage = message;
                TempData.Remove("Message");
                TempData["Message"] = null;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(User inputUser, IFormFile photo)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                string pathPhoto = null;
                user.PhoneNumber = inputUser.PhoneNumber;
                user.Cpf = inputUser.Cpf;
                user.FirstName = inputUser.FirstName;
                user.LastName = inputUser.LastName;
                if (photo != null)
                {
                    DateTime getNow = DateTime.Now;
                    string now = getNow.ToString().Replace(" ", "").Replace("/", "").Replace(":", "");
                    string pathFile = Path.Combine(_webHostEnvironment.WebRootPath, "Img/UserProfilePictures");
                    string fileName = now + "-User_" + user.Id.ToString() + ".png";
                    using (FileStream fileStream = new(Path.Combine(pathFile, fileName), FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                        pathPhoto = "~/Img/UserProfilePictures/" + fileName;
                    }
                    user.PathPhoto = pathPhoto;
                }

                await _userService.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Seu perfil foi atualizado!";
                return RedirectToPage();
            }
            catch (Exception)
            {
                StatusMessage = "Erro - algo deu errado ao Atualizar seu perfil.";
                return RedirectToPage();
            }
        }
    }
}
