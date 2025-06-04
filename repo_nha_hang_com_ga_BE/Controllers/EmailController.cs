using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models;
using repo_nha_hang_com_ga_BE.Services;

namespace repo_nha_hang_com_ga_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _emailService.SendEmailAsync(emailModel);
            if (result)
            {
                return Ok(new { message = "Email sent successfully" });
            }

            return BadRequest(new { message = "Failed to send email" });
        }
    }
}