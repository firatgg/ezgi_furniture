using ezgi_mobilya.Models;
using ezgi_mobilya.Service.DTOs;
using ezgi_mobilya.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ezgi_mobilya.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IContactMessageService _contactMessageService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            IContactMessageService contactMessageService)
        {
            _logger = logger;
            _productService = productService;
            _contactMessageService = contactMessageService;
        }

        public async Task<IActionResult> Index()
        {
            // Active products are retrieved from database
            var activeProducts = await _productService.GetActiveProductsAsync();
            return View(activeProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContactMessage(ContactMessageDto contactMessageDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _contactMessageService.AddAsync(contactMessageDto);
                    TempData["SuccessMessage"] = "Mesajınız başarıyla iletildi. En kısa sürede sizinle iletişime geçeceğiz.";
                    return RedirectToAction(nameof(Index), "Home", "#contact");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İletişim formu kaydedilirken hata oluştu.");
                    TempData["ErrorMessage"] = "Mesajınız gönderilirken bir hata oluştu. Lütfen tekrar deneyiniz.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Lütfen form alanlarını doğru doldurduğunuzdan emin olun.";
            }

            return RedirectToAction(nameof(Index), "Home", "#contact");
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
