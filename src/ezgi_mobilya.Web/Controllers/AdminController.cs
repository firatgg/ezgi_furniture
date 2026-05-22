using ezgi_mobilya.Service.DTOs;
using ezgi_mobilya.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IProductService productService,
            ICategoryService categoryService,
            IContactMessageService contactMessageService,
            ISocialMediaService socialMediaService,
            ILogger<AdminController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _contactMessageService = contactMessageService;
            _socialMediaService = socialMediaService;
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
    }
}
