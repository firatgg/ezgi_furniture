using ezgi_mobilya.Models;
using ezgi_mobilya.Service.DTOs;
using ezgi_mobilya.Service.Services;
using ezgi_mobilya.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ezgi_mobilya.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IContactMessageService _contactMessageService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            IContactMessageService contactMessageService,
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _productService = productService;
            _contactMessageService = contactMessageService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            // Active products are retrieved from database
            var activeProducts = await _productService.GetActiveProductsAsync();
            return View(activeProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContactMessage([Bind("FullName,Email,Subject,Message")] ContactMessageDto contactMessageDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _contactMessageService.AddAsync(contactMessageDto);
                    TempData["SuccessMessage"] = _localizer["Contact_Success"].Value;
                    return RedirectToContact();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İletişim formu kaydedilirken hata oluştu.");
                    TempData["ErrorMessage"] = _localizer["Contact_Error"].Value;
                }
            }
            else
            {
                _logger.LogWarning(
                    "İletişim formu doğrulanamadı. Hatalar: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                TempData["ErrorMessage"] = _localizer["Contact_Invalid"].Value;
            }

            return RedirectToContact();
        }

        private RedirectToActionResult RedirectToContact()
            => RedirectToAction(nameof(Index), "Home", fragment: "contact");

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
