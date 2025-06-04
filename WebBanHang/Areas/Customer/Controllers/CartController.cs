using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Areas.Customer.Models;
using WebBanHang.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    { 
            private readonly ApplicationDbContext _db;
            public CartController(ApplicationDbContext db)
            {
                _db = db;
            }
            // hiển thị giao diện quản lý giỏ hàng
            public IActionResult Index()
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("CART");
                if (cart == null)
                {
                    cart = new Cart();
                }
                return View(cart);
            }
            //thêm sản phẩm vào giỏ hàng
            public IActionResult AddToCart(int productId)
            {
                var product = _db.Products.FirstOrDefault(x => x.Id == productId);
                if (product != null)
                {
                    Cart cart = HttpContext.Session.GetJson<Cart>("CART");
                    if (cart == null)
                    {
                        cart = new Cart();
                    }
                    cart.Add(product, 1);
                    HttpContext.Session.SetJson("CART", cart);
                    
                    return RedirectToAction("Index");
                }
                return Json(new { msg = "error" });
            }
            // cập nhật số lượng
            public IActionResult Update(int productId, int qty)
            {
                var product = _db.Products.FirstOrDefault(x => x.Id == productId);
                if (product != null)
                {
                    //lấy cart từ Session
                    Cart cart = HttpContext.Session.GetJson<Cart>("CART");
                    if (cart != null)
                    {
                        cart.Update(productId, qty); 
                        HttpContext.Session.SetJson("CART", cart); 
                        return RedirectToAction("Index");
                    }
                }
                return Json(new { msg = "error" });
            }
            // xoá sản phẩm trong giỏ hàng
            public IActionResult Remove(int productId)
            {
                var product = _db.Products.FirstOrDefault(x => x.Id == productId);
                if (product != null)
                {
                    Cart cart = HttpContext.Session.GetJson<Cart>("CART");
                    if (cart != null)
                    {
                        cart.Remove(productId); 
                        HttpContext.Session.SetJson("CART", cart); 
                        return RedirectToAction("Index");
                    }
                }
                return NotFound();
            }
        #region API
        public IActionResult ThongKeSL()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("CART");
            if(cart != null)
            {
                return Json(new { qty = cart.Quantity });
            }
            return Json(new { qty = 0 });
        }
        public IActionResult GetQuantityOfCart()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("CART");
            if (cart != null)
            {
                return Json(new { qty = cart.Quantity });

            }
            return Json(new { qty = 0 });
        }
        #endregion
    }
}
