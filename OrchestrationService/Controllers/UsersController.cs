using Microsoft.AspNetCore.Mvc;
using OrchestrationService.Shared;
using OrchestrationService.Model.Requests;
using OrchestrationService.Services.Authorization;

namespace OrchestrationService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService service) : ControllerBase
{
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
                if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
                
                var res = await service.Login(request);
                
                return FromResult(res);
        }

        /*[HttpPost]
        [Authorize]
        [Route("logout")]
        public IActionResult Logout()
        {
                return Ok();
        }*/

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
                if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
                
                var res = await service.Register(request);
                
                return FromResult(res);
        }

        /*[HttpPost]
        [Authorize]
        [Route("refresh")]
        public IActionResult RefreshToken()
        {
                return Ok();
        }
        */
        
        private IActionResult FromResult<T>(Result<T> result)
        {
                if (result.IsSuccess) return Ok(new { data = result.Value });

                return result.Status switch
                {
                        ErrorStatus.ValidationError => BadRequest(new { message = result.ErrorMessage }),
                        ErrorStatus.Conflict => Conflict(new { message = result.ErrorMessage }),
                        ErrorStatus.NotFound => NotFound(new { message = result.ErrorMessage }),
                        ErrorStatus.Unauthorized => Unauthorized(new { message = result.ErrorMessage }),
                        ErrorStatus.Forbidden => Forbid(),
                        ErrorStatus.ServerError => StatusCode(500, new { message = result.ErrorMessage }),
                        _ => BadRequest(new { message = result.ErrorMessage })
                };
        }
}