﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;


        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");

            // Find out id of the logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (c => c.ApplicationUserId == claim.Value).ToList().Count();
                
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties:"Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id,
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                // then we will add to cart

                // Find id of logged user - store userId of the logged user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                // retrieve shopping cart from DB based on the userId and productId
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.ProductId == CartObject.ProductId && 
                    c.ApplicationUserId == CartObject.ApplicationUserId, includeProperties:"Product");

                if (cartFromDb == null)
                {
                    // No record exists in database for that user
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }
                else
                {
                    // update the count in db
                    cartFromDb.Count += CartObject.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll
                    (c => c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();

                // add count(number of items of current user) to session - custom metoda koja podrzava dodavanje objekata u session
                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObject); // Može bit i objekt, lista, int(count)
                
                // asp.net core ima build in session samo za integer
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);


                // Primjer kako vratit vrijednost
                //var obj = HttpContext.Session.GetObject<ShoppingCart>(SD.ssShoppingCart);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // return to the view
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == CartObject.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id,
                };
                return View(cartObj);

            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
