using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;
// Assuming a service layer for business logic

namespace StoreAPI.Controllers;
[ApiController]
[Route("api/[controller]")]

public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
    {
        _context = context;
     
    }

        // GET: api/Categories
        [HttpGet]
        public ActionResult<Category> GetCategories()
        {
           
            var categories = _context.categories.ToList();
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _context.categories.FirstOrDefault(p => p.category_id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public ActionResult<Category> UpdateCategory(int id, Category category)
        {
            // ดึงข้อมูลสินค้าตาม id
             var existingCategory = _context.categories.FirstOrDefault(p => p.category_id == id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            existingCategory.category_name = category.category_name;
            existingCategory.category_status = category.category_status;
           
            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Categories
        [HttpPost]
        public  ActionResult<Category> PostCategory(Category category)
        {
           
            _context.categories.Add(category);
             _context.SaveChanges();
            return Ok(category);
            
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public  ActionResult<Category> DeleteCategory(int id)
        {
            var category = _context.categories.FirstOrDefault(p => p.category_id == id);
            if (category == null)
            {
                return NotFound();
            }

          _context.categories.Remove(category);
          _context.SaveChanges();

            // ส่งข้อมูลกลับไปให้ผู้ใช้
            return Ok(category);
        }
    }
