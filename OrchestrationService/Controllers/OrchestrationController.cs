using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OrchestrationService.Services.Orchestration;

namespace OrchestrationService.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrchestrationController(IOrchestrationService service) : ControllerBase
{
    private readonly IOrchestrationService _service = service;

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] ScenarioRequest request)
    {
        var result = await _service.StartScenarioAsync(request.StudentPublicId, request.ScenarioId);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}

public class ScenarioRequest
{
    public string StudentPublicId { get; set; } = default!;
    public string ScenarioId { get; set; } = default!;
}