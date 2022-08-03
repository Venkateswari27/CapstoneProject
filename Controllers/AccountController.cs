using CapstoneProject_.NETFSD.Data;
using CapstoneProject_.NETFSD.Data.Static;
using CapstoneProject_.NETFSD.Data.ViewModels;
using CapstoneProject_.NETFSD.Models;
using CapstoneProject_.NETFSD.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject_.NETFSD.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize(Roles = UserRoles.Admin)] 
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                var PassCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (PassCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Medicines");
                    }
                }
                TempData["Error"] = "Wrong credential, please try again!";
                return View(loginVM);
            }
            TempData["Error"] = "Wrong credential, please try again!";
            return View(loginVM);
        }

        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            var User = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if (User != null) 
            {
                TempData["Error"] = "Email  address is already in use";
                return View(registerVM);           
            }

            var NewUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress
            };
            var NewUserResponse = await _userManager.CreateAsync(NewUser, registerVM.Password);

            if (NewUserResponse.Succeeded)
            { 
                await _userManager.AddToRoleAsync(NewUser, UserRoles.User);
                return View("RegisterCompleted");
            }
            TempData["Error"] = "Password must contain: At least 6 characters, OneUpperCase, OneLowerCase, OneNumber, Symbol as [$#@]"; // [TroubleShooting] when seed pass shoud provide asp.netPass_requiremen [PassValidation] MSH
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Medicines");

        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        } 
        public IActionResult DemoCredentials()
        {
            return View();
        }


    }
}
