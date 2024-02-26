using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sputter.Messaging;

namespace Sputter.Server.Controllers;

[ApiController]
[Route("drives")]
public class DrivesDiscoveryController(ILogger<DrivesDiscoveryController> logger, IMediator mediator) : ControllerBase
{
    [HttpGet(Name = "GetAllDrives")]
    public async Task<IActionResult> Get([FromServices]DriveSpecificationReader driveReader)
    {
        var templates = driveReader.GetSpecifications();
        logger.LogDebug("Found {Count} templates: {templates}", templates.Count, string.Join(';', templates.Select(t => t.ToString())));
        var req = new DriveDiscoveryRequest() { Templates = templates };
        var res = await mediator.Send(req, HttpContext.RequestAborted);
        var drives = res.ToList();
        return Ok(drives);
    }
}
