using TypicalTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly DBContext context;

        public ProductController(DBContext dBContext)
        {
            context = dBContext;
        }

        // Show all products
        public IActionResult Index()
        {
            var products = context.GetProducts();
            return View(products);
        }

        /**
         * Requires administrator role on user to display page, else redirects to login/unauthorised page
         */
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct()
        {
            Product product = new Product();
            return View(product);
        }

        /**
         * Adds product to database (Only accessible if user is authorised & administrator)
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                context.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ErrorMessage = "The product entered is invalid!";
            return View(product);            
            
        }
        /**
         * Requires user to have administrator role to view page for editing price, else redirects to login/unauthorised
         */
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int productId)
        {
            if (productId == null)
            {
                return NotFound();
            }
            Product product = context.GetSingleProduct(productId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        /**
         * Double checks user is authentication and the model state is valid before updating product price, 
         * Else returns to index
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Check if the admin is logged in
            string authStatus = HttpContext.Session.GetString("Authenticated");
            bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("True");

            if (isAdmin && ModelState.IsValid)
            {
                product.updated_date = DateTime.Now;
                context.EditProduct(product);
            }

            return RedirectToAction("Index", "Product", new { id = product.id });

        }


    }
}
