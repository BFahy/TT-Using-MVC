using TypicalTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TypicalTools.Controllers
{
    public class AdminController : Controller
    {            
        private readonly DBContext context;

        public AdminController(DBContext context)
        {
            this.context = context;
        }

        /**
         * Page displayed when user is unauthorised
         */
        public IActionResult AccessDenied()
        {
            return View();
        }

        /*
         * Clearing session information and signing out
         */
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        /**
         * Login page used for redirects or to authenticate on login
         */
        public IActionResult Login()
        {
            return View();
        }


        /*
         * Checking model is valid -> 
         * Setting up identity/session info for username, role and authentication check
         */
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                User logged = context.CheckLogin(user);
                if (logged != null)
                {
                    HttpContext.Session.SetString("User", logged.username);
                    HttpContext.Session.SetString("Role", logged.role);
                    HttpContext.Session.SetString("Authenticated", "True");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, logged.username),
                        new Claim(ClaimTypes.Role, logged.role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), 
                        authProperties);

                    return RedirectToAction("Index", "Product");
                }
            }
            ViewBag.ErrorMessage = "Invalid username or password";
            return View(user);
        }

        /*
         * Get request to display registration page
         */
        public IActionResult Register()
        {
            return View();
        }

        /*
         * Checking username is unique -> password and username match
         * Creates user in database
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (user.password.Equals(user.salt) == false)
            {
                ViewBag.ErrorMessage = "Make sure password and confirmation match.";
                return View(user);
            }

            user.role = "User";

            bool status = context.CreateAccount(user);

            if (status)
            {
                return RedirectToAction("Index", "Product");
            }

            ViewBag.ErrorMessage = "Username Already Exists.";
            return View(user);
        }


    }
}
