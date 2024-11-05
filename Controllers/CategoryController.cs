using E_Tickets.Controllers;
using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using E_Tickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole},{SD.companyRole}")]
    public class CategoryController : Controller
    {
       
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        private readonly ICategoryRepository categoryRepository;

        public IActionResult Index()
        {
            var categories = categoryRepository.Get([e => e.Movies], tracked: false).ToList();

           // var categories = dbContext.Categories.Include(e => e.Movies).ToList();
            return View(model: categories);
        }
        public IActionResult Create()
        {
            Category category = new Category();
            return View(category);
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Create(category);
                categoryRepository.Commit();
                TempData["success"] = "Category Added Successfully";
                return RedirectToAction(nameof(Index));

                
            }

            return View(category);
        }

        public IActionResult Edit(int categoryId)
        {
            var category = categoryRepository.GetOne(expression: e => e.Id == categoryId);
            return View(model: category);

           
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                //Category category = new() { Id = Id, Name = Name };
                categoryRepository.Edit(category);
                categoryRepository.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        public IActionResult Delete(int categoryId)
        {
            var category=categoryRepository.GetOne(expression:e => e.Id == categoryId);
           
            if (category == null)
                return RedirectToAction("NotFound", "Home");

            categoryRepository.Delete(category);
            categoryRepository.Commit();
            return RedirectToAction(nameof(Index));
            
        }
    }
}

