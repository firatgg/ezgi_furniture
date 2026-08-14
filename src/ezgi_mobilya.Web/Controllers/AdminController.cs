using ezgi_mobilya.Service.DTOs;
using ezgi_mobilya.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ezgi_mobilya.Web.Controllers
{
    // The Admin Panel is protected. Only users in the "Admin" role can access.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IContactMessageService _contactMessageService;
        private readonly ISocialMediaService _socialMediaService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            IContactMessageService contactMessageService,
            ISocialMediaService socialMediaService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<AdminController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _contactMessageService = contactMessageService;
            _socialMediaService = socialMediaService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // 1. DASHBOARD
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var messages = await _contactMessageService.GetAllAsync();
            var posts = await _socialMediaService.GetAllPostsAsync();

            ViewBag.ProductCount = products.Count;
            ViewBag.CategoryCount = categories.Count;
            ViewBag.MessageCount = messages.Count;
            ViewBag.SocialPostCount = posts.Count;

            return View(posts);
        }

        // 2. PRODUCTS LIST
        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetProductsWithCategoryAsync();
            return View(products);
        }

        // 3. MESSAGES (Inbox)
        public async Task<IActionResult> Messages()
        {
            var messages = await _contactMessageService.GetAllAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessageAsRead(int id)
        {
            try
            {
                await _contactMessageService.MarkAsReadAsync(id);
                TempData["AdminSuccess"] = "Mesaj okundu olarak işaretlendi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj okundu işaretlenirken hata oluştu.");
                TempData["AdminError"] = "İşlem sırasında bir hata oluştu.";
            }
            return RedirectToAction(nameof(Messages));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                await _contactMessageService.DeleteAsync(id);
                TempData["AdminSuccess"] = "Mesaj silindi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj silinirken hata oluştu.");
                TempData["AdminError"] = "Mesaj silinemedi.";
            }
            return RedirectToAction(nameof(Messages));
        }

        // 4. SOCIAL MEDIA SCHEDULER & AUTO-POST
        public async Task<IActionResult> SocialMedia()
        {
            var posts = await _socialMediaService.GetAllPostsAsync();
            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishSocialPost(SocialPostDto postDto, string[] selectedPlatforms)
        {
            if (selectedPlatforms == null || selectedPlatforms.Length == 0)
            {
                ModelState.AddModelError("", "Lütfen en az bir sosyal medya platformu seçiniz.");
                TempData["AdminError"] = "Lütfen en az bir platform seçin (Instagram/Facebook).";
                return RedirectToAction(nameof(SocialMedia));
            }

            postDto.Platforms = string.Join(",", selectedPlatforms);

            if (string.IsNullOrEmpty(postDto.Caption))
            {
                TempData["AdminError"] = "Gönderi metni (açıklama) boş bırakılamaz.";
                return RedirectToAction(nameof(SocialMedia));
            }

            try
            {
                var result = await _socialMediaService.CreateAndPublishPostAsync(postDto);
                if (result.IsPosted)
                {
                    TempData["AdminSuccess"] = "Gönderiniz seçilen platformlarda başarıyla paylaşıldı!";
                }
                else
                {
                    TempData["AdminError"] = $"Gönderi paylaşılamadı: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sosyal medya gönderisi yayınlanırken hata oluştu.");
                TempData["AdminError"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(SocialMedia));
        }

        // 5. CREATE PRODUCT (GET)
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(new ProductDto { IsActive = true });
        }

        // 5. CREATE PRODUCT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductDto productDto, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        productDto.ImageUrl = "images/products/" + uniqueFileName;
                        productDto.ThumbnailUrl = "images/products/" + uniqueFileName;
                    }
                    else
                    {
                        productDto.ImageUrl = "themes/infinite-loop/img/toy-gallery-thumb-01.jpg";
                        productDto.ThumbnailUrl = "themes/infinite-loop/img/toy-gallery-thumb-01.jpg";
                    }

                    await _productService.AddAsync(productDto);
                    TempData["AdminSuccess"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ürün eklenirken hata oluştu.");
                    TempData["AdminError"] = "Ürün eklenirken bir hata oluştu.";
                }
            }

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(productDto);
        }

        // 6. EDIT PRODUCT (GET)
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["AdminError"] = "Ürün bulunamadı.";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(product);
        }

        // 6. EDIT PRODUCT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductDto productDto, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Delete old file if it exists and is not a default/external one
                        if (!string.IsNullOrEmpty(productDto.ImageUrl) && !productDto.ImageUrl.StartsWith("http") && !productDto.ImageUrl.StartsWith("themes"))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, productDto.ImageUrl.Replace("/", "\\"));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        productDto.ImageUrl = "images/products/" + uniqueFileName;
                        productDto.ThumbnailUrl = "images/products/" + uniqueFileName;
                    }

                    await _productService.UpdateAsync(productDto);
                    TempData["AdminSuccess"] = "Ürün başarıyla güncellendi.";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ürün güncellenirken hata oluştu.");
                    TempData["AdminError"] = "Ürün güncellenirken bir hata oluştu.";
                }
            }

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(productDto);
        }

        // 7. DELETE PRODUCT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageUrl) && !product.ImageUrl.StartsWith("http") && !product.ImageUrl.StartsWith("themes"))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.Replace("/", "\\"));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    await _productService.DeleteAsync(id);
                    TempData["AdminSuccess"] = "Ürün başarıyla silindi.";
                }
                else
                {
                    TempData["AdminError"] = "Ürün bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün silinirken hata oluştu.");
                TempData["AdminError"] = "Ürün silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Products));
        }
    }
}
