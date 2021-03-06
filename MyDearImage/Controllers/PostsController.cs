#nullable disable
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnvironment = hostEnvironment;
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

        public async Task<IActionResult> MyPosts()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return NoContent();
            }
            else
            {
                var posts = from p in _context.Post select p;
                var user = await _userManager.GetUserAsync(HttpContext.User);

                posts = posts.Where(p => p.UserId == user.Id);

                return View(await posts.ToListAsync());
            }
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Title,FormFile,Description")] Post post)
        {
            Account account = new Account("imagedpy", "882864429614789", "J0ISV-xrcX_pod7fhdyLSJ06Gl4");
            Cloudinary cloudinary = new Cloudinary(account);
            string filename = post.FormFile.FileName;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription($@"D:/Computador/Images/{filename}"),
                PublicId = filename.Replace(".jpg", ""),
                UploadPreset = "bxmouqwf",
                Folder = "myddearimage", 
                ImageMetadata = true
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            if (ModelState.IsValid)
            {
                post.Image = uploadResult.SecureUrl.OriginalString;
                post.CreatedDate = DateTime.Now;
                post.UserId = user.Id;
                post.UserName = user.FirstName + " " + user.LastName;
                post.LikeCount = 0;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public async Task<IActionResult> LikePhoto(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var post = await _context.Post.FirstOrDefaultAsync(m => m.Id == id);
                post.LikeCount++;
                _context.Update(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,FormFile,Description")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            Account account = new Account("imagedpy", "882864429614789", "J0ISV-xrcX_pod7fhdyLSJ06Gl4");
            Cloudinary cloudinary = new Cloudinary(account);
            string filename = post.FormFile.FileName;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription($@"D:/Computador/Images/{filename}"),
                PublicId = filename.Replace(".jpg", ""),
                UploadPreset = "bxmouqwf",
                Folder = "myddearimage",
                ImageMetadata = true,
                
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            if (ModelState.IsValid)
            {
                
                try
                {
                    post.Image = uploadResult.SecureUrl.OriginalString;
                    post.CreatedDate = DateTime.Now;
                    post.UserId = user.Id;
                    post.UserName = user.UserName;
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
                return RedirectToAction(nameof(MyPosts));
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
            return RedirectToAction(nameof(MyPosts));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
