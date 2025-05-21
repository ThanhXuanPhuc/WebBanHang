using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using X.PagedList;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }   
        public IActionResult Index(int page=1)
        {
            int pageSize = 3;
            var currentPage = page;
            var dsSanPham = _db.Products.Include(x => x.Category).ToList();
            //Truyen du lieu cho View
            ViewBag.PageSum = Math.Ceiling((double)dsSanPham.Count / pageSize);
            ViewBag.CurrentPage = currentPage;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") 
            {
                return PartialView("ProductPartial", dsSanPham.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
            }
            return View(dsSanPham.Skip((currentPage-1)*pageSize).Take(pageSize).ToList());
        }
        public IActionResult Add()
        {
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        [HttpPost]
        public IActionResult Add(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    product.ImageUrl = SaveImage(ImageUrl);
                }
                _db.Products.Add(product);
                _db.SaveChanges();
                TempData["success"] = "Product inserted success";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        public IActionResult Update(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {

                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _db.Products.Find(product.Id);
                if (existingProduct == null)
                {
                    TempData["update_error"] = "Product not found";
                    return RedirectToAction("Index");
                }
                try
                {
                    if (ImageUrl != null)
                    {
                        product.ImageUrl = SaveImage(ImageUrl);
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            var oldFilePath = Path.Combine(_hosting.WebRootPath, existingProduct.ImageUrl);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.ImageUrl = product.ImageUrl;

                    _db.SaveChanges();
                    TempData["update_success"] = "Product updated success";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["update_error"] = "Product update failed";
                }
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(product);
        }
        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                TempData["delete_error"] = "Product not found";
                return RedirectToAction("Index");
            }
            try
            {
                if (!String.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                _db.Products.Remove(product);
                _db.SaveChanges();
                TempData["delete_success"] = "Product deleted success";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["delete_error"] = "Product delete failed";
                return RedirectToAction("Index");
            }

        }
    }
}
