using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Repositories;
using BethanysPieShopAdmin.Models.ViewModels;
using BethanysPieShopAdmin.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BethanysPieShopAdmin.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;

        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            _pieRepository = pieRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var pies = await _pieRepository.GetAllPiesAsync();
            return View(pies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pie = await _pieRepository.GetPieByIdAsync(id);
            return View(pie);
        }

        public async Task<IActionResult> Add()
        {
            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories,
                "CategoryId", "Name", null);
            PieAddViewModel pieViewModel = new() { Categories = selectListItems };

            return View(pieViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PieAddViewModel pieAddViewModel)
        {
            if (ModelState.IsValid && pieAddViewModel != null)
            {
                var pie = pieAddViewModel.Pie;
                await _pieRepository.AddPieAsync(pie);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var allCategories = await _categoryRepository.GetAllCategoriesAsync();
                IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories,
                    "CategoryId", "Name", null);

                pieAddViewModel.Categories = selectListItems;

                return View(pieAddViewModel);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            var selectedPie = await _pieRepository.GetPieByIdAsync(id.Value);

            PieEditViewModel pieEditViewModel = new() { Categories = selectListItems, Pie = selectedPie };
            return View(pieEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(PieEditViewModel pieEditViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _pieRepository.UpdatePieAsync(pieEditViewModel.Pie);
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

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            pieEditViewModel.Categories = selectListItems;

            return View(pieEditViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var selectedCategory = await _pieRepository.GetPieByIdAsync(id);

            return View(selectedCategory);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? pieId)
        {
            if (pieId == null)
            {
                ViewData["ErrorMessage"] = "Deleting the pie failed, invalid ID!";
                return View();
            }

            try
            {
                await _pieRepository.DeletePieAsync(pieId.Value);
                TempData["PieDeleted"] = "Pie deleted successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Deleting the pie failed, please try again! Error: {ex.Message}";
            }

            var selectedPie = await _pieRepository.GetPieByIdAsync(pieId.Value);
            return View(selectedPie);
        }

        private int pageSize = 5;

        public async Task<IActionResult> IndexPaging(int? pageNumber)
        {
            var pies = await _pieRepository.GetPiesPagedAsync(pageNumber, pageSize);
            pageNumber ??= 1;

            var count = await _pieRepository.GetAllPiesCountAsync();

            return View(new PaginatedList<Pie>(pies.ToList(),count,pageNumber.Value,pageSize));
        }

        public async Task<IActionResult> IndexPagingSorting(string sortBy, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortBy;

            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortBy) || sortBy == "id_desc" ? "id" : "id_desc";
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortBy) || sortBy == "name_desc" ? "name" : "name_desc";
            ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortBy) || sortBy == "price_desc" ? "price" : "price_desc";

            var pies = await _pieRepository.GetPiesSortedAndPagedAsync(sortBy, pageNumber, pageSize);
            pageNumber ??= 1;

            var count = await _pieRepository.GetAllPiesCountAsync();

            return View(new PaginatedList<Pie>(pies.ToList(),count,pageNumber.Value,pageSize));
        }

    }
}
