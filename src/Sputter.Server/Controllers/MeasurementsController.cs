using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sputter.Core;
using Sputter.Messaging;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.Server.Controllers;

[ApiController]
[Route("measurements")]
public class MeasurementsController(ILogger<MeasurementsController>? logger, IMediator mediator) : ControllerBase {

    [HttpGet("{adapter:adapter}/{filter?}", Name = "GetDriveMeasurementsByAdapter")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, DriveMeasurement?>))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "ASP0018:Unused route parameter", Justification = "Analyzer bug: https://github.com/dotnet/aspnetcore/issues/54212")]
    public async Task<IActionResult> GetBySpecificAdapter([FromRoute]string adapter, MeasurementsRequestModel request) {
        if (string.IsNullOrWhiteSpace(adapter)) return BadRequest();
        logger?.LogDebug("Getting measurements for specific adapter '{Adapter}' with filter: '{Filter}'", adapter, request.Filter);
        var template = new DiscoveryTemplate(request.Filter ?? "*") { SourceAdapter = adapter };
        var driveReq = new DriveDiscoveryRequest() { Templates = [template] };
        var drives = await mediator.Send(driveReq, HttpContext.RequestAborted);
        if (drives == null) return StatusCode(412);
        var req = new DriveMeasurementRequest(drives);
        var res = await mediator.Send(req, HttpContext.RequestAborted);
        //if ((publish == true || publishHeader == true) && (publish != false && publishHeader != false)) {
        if (request.EnablePublishing) { 
            var notif = new DriveMeasurementNotification(res);
            await mediator.Publish(notif, HttpContext.RequestAborted);
        }
        return Ok(res.ToDiskDictionary(request.UseSerialNumbers));
    }

	//  [HttpGet("{filter?}", Name = "GetDriveMeasurements")]
	//  [ProducesResponseType(200, Type = typeof(Dictionary<string, DriveMeasurement?>))]
	//  //public async Task<IActionResult> GetMeasurements(MeasurementsRequestModel requestModel) {
	//  public async Task<IActionResult> GetMeasurements(
	//      [FromRouteAndQuery] string? filter,
	//      [FromQuery] bool useSerialNumbers = false,
	//      [FromQueryAndHeader] bool? enablePublishing = null,
	//[FromServices] DriveSpecificationReader? driveReader = null
	//) {
	//if (string.IsNullOrWhiteSpace(filter) && driveReader != null) {
	//	var templates = driveReader.GetSpecifications();
	//	var res = await _mediator.MeasureDrives(templates, HttpContext.RequestAborted, enablePublishing == true);
	//	return Ok(res.ToDiskDictionary(useSerialNumbers));
	//} else {
	//	var res = await _mediator.MeasureDrives(filter, HttpContext.RequestAborted, enablePublishing == true);
	//	return Ok(res.ToDiskDictionary(useSerialNumbers));
	//}
	//  }

	[HttpGet("{filter?}", Name = "GetDriveMeasurements")]
	[ProducesResponseType(200, Type = typeof(Dictionary<string, DriveMeasurement?>))]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "ASP0018:Unused route parameter", Justification = "Analyzer bug: https://github.com/dotnet/aspnetcore/issues/54212")]
	public async Task<IActionResult> GetMeasurements(
		[Bind] MeasurementsRequestModel requestModel,
		[FromServices] DriveSpecificationReader? driveReader = null) {
        if (string.IsNullOrWhiteSpace(requestModel.Filter) && driveReader != null) {
            var templates = driveReader.GetSpecifications();
            if (templates != null && templates.Count > 0) {
                logger?.LogDebug("Loaded {TemplateCount} from template reader, starting measurement gathering", templates.Count);
                var res = await mediator.MeasureDrives(templates, HttpContext.RequestAborted, requestModel.EnablePublishing, logger: logger);
                return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
            } else {
                logger?.LogDebug("Loaded drive filter from request, starting measurement gathering");
                var res = await mediator.MeasureDrives(requestModel.Filter, HttpContext.RequestAborted, requestModel.EnablePublishing, logger: logger);
                return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
            }
            
        } else {
            logger?.LogDebug("Falling back to legacy discovery and measurement request behaviour");
            var res = await mediator.MeasureDrives(requestModel.Filter, HttpContext.RequestAborted, requestModel.EnablePublishing, logger: logger);
            return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
        }

    //if (string.IsNullOrWhiteSpace(requestModel.Filter) && driveReader != null) {
    //	// no filter, and config reader is available
    //	// filter falls back to configured defaults
    //	var templates = driveReader.GetSpecifications();
    //	var res = await _mediator.MeasureDrives(templates, HttpContext.RequestAborted, requestModel.EnablePublishing);
    //	return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
    //} else if (!string.IsNullOrWhiteSpace(requestModel.Filter) && templateParser != null) {
    //	// filter provided, parser is available
    //	// parse the provided filter for a real template
    //	var template = templateParser.ParseTemplate(requestModel.Filter);
    //	if (template == null) {
    //		// filter parsing failed
    //		_logger?.LogWarning("Failed to process filter as template, falling back to legacy behaviour!");
    //		var res = await _mediator.MeasureDrives(requestModel.Filter, HttpContext.RequestAborted, requestModel.EnablePublishing);
    //		return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
    //	} else {
    //		// filter parsing succeeded, we can use the parsed template instead
    //		var res = await _mediator.MeasureDrives([template], HttpContext.RequestAborted, requestModel.EnablePublishing);
    //		return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
    //	}
    //} else {
    //	// either no filter and no config reader, or filter but no parser available
    //	var res = await _mediator.MeasureDrives(requestModel.Filter, HttpContext.RequestAborted, requestModel.EnablePublishing);
    //	return Ok(res.ToDiskDictionary(requestModel.UseSerialNumbers));
    //}
}
}
