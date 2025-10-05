﻿using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Repositories;
using BethanysPieShopAdmin.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BethanysPieShopAdmin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                CategoryListViewModel model = new CategoryListViewModel
                {
                    Categories = (await _categoryRepository.GetAllCategoriesAsync()).ToList()
                };
                return View(model);
            }
            catch(Exception ex)
            {
                ViewData["Error Message"] = $"Error occured: {ex.Message}";
            }


            return View(new CategoryListViewModel());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var category = await _categoryRepository.GetCategoryByIdAsync(id.Value);
            return View(category);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([Bind ("Name,Description,DateAdded")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryRepository.AddCategoryAsync(category);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Adding the category failed please try again! Error: {ex.Message}");
                    return View(category);
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var selectedCategory = await _categoryRepository.GetCategoryByIdAsync(id.Value);
            return View(selectedCategory);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryRepository.UpdateCategoryAsync(category);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Updating the category failed, please try again! Error: {ex.Message}");
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var selectedCategory = await _categoryRepository.GetCategoryByIdAsync(id);

            return View(selectedCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? CategoryId)
        {
            if (CategoryId == null)
            {
                ViewData["ErrorMessage"] = "Deleting the category failed, invalid ID!";
                return View();
            }

            try
            {
                await _categoryRepository.DeleteCategoryAsync(CategoryId.Value);
                TempData["CategoryDeleted"] = "Category deleted successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Deleting the category failed, please try again! Error: {ex.Message}";
            }

            var selectedCategory = await _categoryRepository.GetCategoryByIdAsync(CategoryId.Value);
            return View(selectedCategory);

        }

        public async Task<IActionResult> BulkEdit()
        {
            List<CategoryBulkEditViewModel> categoryBulkEditViewModels = new List<CategoryBulkEditViewModel>();

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();
            foreach(var category in allCategories)
            {
                categoryBulkEditViewModels.Add(new CategoryBulkEditViewModel
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                });
            }
            return View(categoryBulkEditViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> BulkEdit(List<CategoryBulkEditViewModel> categoryBuldEditViewModels)
        {
            
            List<Category> categories = new List<Category>();

            foreach(var categoryVm in categoryBuldEditViewModels)
            {
                categories.Add(new Category
                {
                    CategoryId = categoryVm.CategoryId,
                    Name = categoryVm.Name
                });
            }

            await _categoryRepository.UpdateCategoryNamesAsync(categories);

            return RedirectToAction(nameof(Index));

        }

    }
}
