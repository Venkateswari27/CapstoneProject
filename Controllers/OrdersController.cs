using CapstoneProject_.NETFSD.Data.Cart;
using CapstoneProject_.NETFSD.Data.Services;
using CapstoneProject_.NETFSD.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CapstoneProject_.NETFSD.Controllers
{
    [Authorize]                                          
    public class OrdersController : Controller
    { 
        private readonly IMedicinesService _medicinesService;
        private readonly IOrdersService _ordersService;

        private readonly ShoppingCart _ShoppingCart;
        
        public OrdersController(IMedicinesService medicinesService, IOrdersService ordersService, ShoppingCart shoppingCart)
        {
            _medicinesService = medicinesService;
            _ShoppingCart = shoppingCart;
            _ordersService = ordersService;
        }

        // Get Orders
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);
            var Orders = await _ordersService.GetOrdersByUserIdAndRole(userId, userRole);
            return View(Orders);
        }                                                   

        [AllowAnonymous]                                   
        public IActionResult ShoppingCart()
        {
            var Items = _ShoppingCart.GetShoppingCartItems();
            _ShoppingCart.ShoppingCartItems = Items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _ShoppingCart,
                ShoppingCartTotal = _ShoppingCart.GetShoppingCartTotal()
            };
            return View(response);
        }
        
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _medicinesService.GetByID(id);
            if (item != null)
            {
                _ShoppingCart.AddItemToCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart)); 
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _medicinesService.GetByID(id);
            if (item != null)
            {
                _ShoppingCart.DeleteItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart)); 
        }
        public async Task<IActionResult> CompleteOrder()
        {
            var items = _ShoppingCart.GetShoppingCartItems();

            if (items.Count!=0)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

                await _ordersService.StoreOrder(items, userId, userEmailAddress);
                await _ShoppingCart.ClearShoppingCart();

                return View();
            }
            TempData["Error"] = "Trespassing detected, cart must contain item to complete order";
            return RedirectToAction(nameof(ShoppingCart));

        }                                               
    }
}
