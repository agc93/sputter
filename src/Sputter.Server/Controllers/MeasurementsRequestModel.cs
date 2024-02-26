using Microsoft.AspNetCore.Mvc;
using Sputter.Server.Infrastructure;

namespace Sputter.Server.Controllers;

public class MeasurementsRequestModel {

	//[FromRouteAndQuery]
    [FromRoute(Name = "filter")]
    public string? Filter { get; set; } = null;

    [FromQuery]
    public bool UseSerialNumbers { get; set; } = false;

    [FromQuery(Name = "publish")]
    public bool? RoutePublish { get; set; }

    [FromHeader(Name = "X-Publish-Results")]
    public bool? HeaderPublish { get; set; }

    internal bool EnablePublishing =>
        (RoutePublish == true || HeaderPublish == true) && (RoutePublish != false && HeaderPublish != false);
}
