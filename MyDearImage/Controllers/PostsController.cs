#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDearImage.Areas.Identity.Data;
using MyDearImage.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace MyDearImage.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PostsController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _signInManager = signInManager;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString)
        {
            if(!_signInManager.IsSignedIn(User))
            {
                    return NoContent();
            }
            else
            {
                var posts = from p in _context.Post
                             select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    posts = posts.Where(s => s.Title!.Contains(searchString) || s.Description.Contains(searchString));
                }

                return View(await posts.ToListAsync());
            }
        }

        public IActionResult Home()
        {
            return View();
        }

        public async Task<IActionResult> MyPosts()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return NoContent();
            }
            else
            {
                return View(await _context.Post.ToListAsync());
            }
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,FormFile,Description,CreatedDate, UserId")] Post post)
        {
            Account account = new Account(
                "imagedpy",
                "882864429614789",
                "J0ISV-xrcX_pod7fhdyLSJ06Gl4");

            Cloudinary cloudinary = new Cloudinary(account);
            
            string filename = post.FormFile.FileName;
            //string pastaFotos = Path.Combine(webHostEnvironment.EnvironmentName);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription($@"D:/Computador/Images/{filename}"),
                PublicId = filename.Replace(".jpg", ""),
                UploadPreset = "bxmouqwf",
                Folder = "myddearimage"
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            if (ModelState.IsValid)
            {
                post.Image = uploadResult.SecureUri.OriginalString;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Image,Description,CreatedDate,UserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
