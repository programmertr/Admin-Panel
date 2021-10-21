using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly BlogContext _context;
        public BlogController(ILogger<BlogController> logger, BlogContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Categories = _context.Category.Select(s =>
            new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }
            ).ToList();
            return View();
        }

        public async Task<IActionResult> Save(Blog blog)
        {
            if(blog != null)
            {
                var file = Request.Form.Files.First();
                //C:\Users\Buse Asena Koca\Desktop\Buse\Blog\MyBlog\MyBlog\wwwroot\img
                string savePath = Path.Combine("C:", "Users", "Buse Asena Koca", "Desktop", "Buse", "Blog", "MyBlog", "MyBlog", "wwwroot","img");
                var fileName = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}";
                var fileUrl = Path.Combine(savePath, fileName);
                using(var fileStream = new FileStream(fileUrl, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                blog.ImagePath = fileName;
                blog.AuthorId = (int)HttpContext.Session.GetInt32("id");
                await _context.AddAsync(blog);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        public IActionResult Login(string Email, string Password)
        {
            var author = _context.Author.FirstOrDefault(e => e.Email == Email
                         && e.Password == Password);
            if (author == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                HttpContext.Session.SetInt32("id", author.Id);
            }
            return RedirectToAction(nameof(Category));
        }

        public IActionResult Author()
        {
            List<Author> list = _context.Author.ToList();
            return View(list);
        }
        public async Task<IActionResult> AddAuthor(Author author)
        {
            var auth = await _context.Author.FindAsync(author.Id);
            if (author.Id == 0)
            {
                await _context.AddAsync(author);
                await _context.SaveChangesAsync();
            }
            else
            {
                auth.Id = author.Id;
                auth.Name = author.Name;
                auth.Surname = author.Surname;
                auth.Email = author.Email;
                auth.Password = author.Password;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Author));
        }
        public async Task<IActionResult> AuthorDetails(int Id)
        {
            var author = await _context.Author.FindAsync(Id);
            return Json(author);
        }
        public async Task<IActionResult> DeleteAuthor(int? Id)
        {
            var author = await _context.Author.FindAsync(Id);
            _context.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Author));
        }
        public async Task<IActionResult> CategoryDetails(int Id)
        {
            var category = await _context.Category.FindAsync(Id);
            return Json(category);
        }

        public IActionResult Category()
        {
            List<Category> list = _context.Category.ToList();
            return View(list);
        }
        public async Task<IActionResult> AddCategory(Category category)
        {
            var cat = await _context.Category.FindAsync(category.Id);
            if (category.Id == 0)
            {
                await _context.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                cat.Id = category.Id;
                cat.Name = category.Name;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Category));
        }
        public async Task<IActionResult> DeleteCategory(int? Id)
        {
            Category category = await _context.Category.FindAsync(Id);
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
