using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int catid = 1)
        {
            var dsLoai = _db.Categories.OrderBy(x => x.DisplayOrder).ToList();
            var dsSanPham = _db.Products.Where(p => p.CategoryId == catid).ToList();
            ViewBag.DSLOAI = dsLoai;
            return View(dsSanPham);
        }
    }
}
