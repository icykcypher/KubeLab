using Microsoft.AspNetCore.Mvc;

namespace OrchestrationService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthorizationController : Controller
{
        [Route("/sing-in")]
        [HttpPost]
        public IActionResult SignIn()
        {
                return Ok();
        }
}