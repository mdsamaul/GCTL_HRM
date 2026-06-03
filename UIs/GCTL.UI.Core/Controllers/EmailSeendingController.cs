using GCTL.Core.ViewModels.EmailSettingsViewModel;
using GCTL.Service.EmailService;
using GCTL.UI.Core.ViewModels.EmailSendingViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class EmailSeendingController : BaseController
    {

        private readonly IEmailService _emailService;

        public EmailSeendingController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            EmailSettingsDTO model = new EmailSettingsDTO()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SendMail([FromBody] EmailRequestDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.ToEmail))
            {
                return Json(new { success = false, message = "Invalid data! Please provide a recipient email." });
            }

            try
            {
                await _emailService.SendEmailAsync(model);
                return Json(new { success = true, message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}