
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            bool exists = await _bethanysPieShopDbContext.Categories.AnyAsync(c=>c.Name == category.Name);
            if(exists)
            {
                throw new Exception($"Category with name {category.Name} already exists.");
            }
            await _bethanysPieShopDbContext.Categories.AddAsync(category);
            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            //throw new Exception("Database is down!!!");
            return _bethanysPieShopDbContext.Categories.OrderBy(c => c.CategoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            throw new Exception("Database is down!!!");
            return await _bethanysPieShopDbContext.Categories.OrderBy(c => c.CategoryId).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _bethanysPieShopDbContext.Categories.
                Include(p => p.Pies).
                FirstOrDefaultAsync(c => c.CategoryId == id);
        }
    }
}
