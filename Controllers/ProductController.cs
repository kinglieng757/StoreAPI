using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;

namespace StroreAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    //สร้าง object ของ applicationDbContext
    private readonly ApplicationDbContext _context;
    // สร้าง Construtor รับค่า ApplicationDbContext
    private readonly IWebHostEnvironment _env;
    public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    //ทดสอบเขียนฟัวก์ชันการเชื่อมต่อ database
    //GET: /api/Product/testconnectdb
    [HttpGet("testconnectdb")]
    public void TEstConnection()
    {
        if (_context.Database.CanConnect())
        {
            Response.WriteAsync("Connected ");
        }
        else 
        {
            Response.WriteAsync("Not Connected ");
        }
    }
    [HttpGet]
    public ActionResult<Product> GetProducts()
    {
        //LINQ สำหรับการดึงข้อมูลตาราง Products ทั้งหมด
        // var products = _context.products.ToList();
        //var products = _context.products.Where(p => p.unit_price >45000).ToList();
        var products = _context.products.Join(_context.categories, p => p.category_id, c => c.category_id,(p,c) => new {
            p.product_id,
            p.product_name,
            p.unit_price,
            p.unit_in_stock,
            c.category_name
        }).ToList();
        return Ok(products);
    }
    [HttpGet("{id}")]
    public ActionResult<Product> GetProducts(int id)
    {
        var products = _context.products.FirstOrDefault(p => p.product_id == id);
        if (products == null)
        {
            return NotFound();
        }
        return Ok(products);
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromForm]Product product, IFormFile image)
    {
        _context.products.Add(product);

        if(image != null){
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string uploadFolder = Path.Combine(_env.ContentRootPath,"uploads");

            if(!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            using ( var fileStream = new FileStream(Path.Combine(uploadFolder,fileName),FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            product.product_picture = fileName;
        }

        _context.SaveChanges();
        return Ok(product);
    }

    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, Product product)
    {
        // ดึงข้อมูลสินค้าตาม id
        var existingProduct = _context.products.FirstOrDefault(p => p.product_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (existingProduct == null)
        {
            return NotFound();
        }

        // แก้ไขข้อมูลสินค้า
        existingProduct.product_name = product.product_name;
        existingProduct.unit_price = product.unit_price;
        existingProduct.unit_in_stock = product.unit_in_stock;
        existingProduct.category_id = product.category_id;

        // บันทึกข้อมูล
        _context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(existingProduct);
    }
     // ฟังก์ชันสำหรับการลบข้อมูลสินค้า
    // DELETE: /api/Product/{id}
    [HttpDelete("{id}")]
    public ActionResult<Product> DeleteProduct(int id)
    {
        // ดึงข้อมูลสินค้าตาม id
        var product = _context.products.FirstOrDefault(p => p.product_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (product == null)
        {
            return NotFound();
        }

        // ลบข้อมูล
        _context.products.Remove(product);
        _context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(product);
    }

}