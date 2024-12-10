using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Areas.DTOs.Category;
using Uniqlo.DAL;
using Uniqlo.Models;

namespace Uniqlo.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }


        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Products.Include(p => p.Category);
            return View(await appDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductDto vm)
        {
            ViewBag.Categories = _context.Categories.ToList();


            if (!ModelState.IsValid)
            {
                return View();
            }
            Product product = new Product()
            {
                Name = vm.Name,
                Description = vm.Description,
                CostPrice = vm.CostPrice,
                Count = vm.Count,
                Price = vm.Price,
                CategoryId = vm.CategoryId
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return View();
            }
            var product = _context.Products.FirstOrDefault(s => s.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            ViewBag.Categories = _context.Categories.ToList();
            if (id == null)
            {
                return View();
            }
            var product = _context.Products.FirstOrDefault(s => s.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product)
        {
            ViewBag.Categories = _context.Categories.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }
            var oldproduct = _context.Products.FirstOrDefault(s => s.Id == product.Id);
            if (oldproduct == null)
            {
                return NotFound();
            }
            oldproduct.Name = product.Name;
            oldproduct.Description = product.Description;
            oldproduct.Price = product.Price;
            oldproduct.CategoryId = product.CategoryId;
            oldproduct.CostPrice = product.CostPrice;
            oldproduct.Count = product.Count;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
