
using Sputter.Core;
using System.Globalization;

namespace Sputter.Server;

public class AdapterRouteConstraint(IEnumerable<IDriveSensorAdapter> adapters) : IRouteConstraint {
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection) {
        if (values.TryGetValue(routeKey, out var routeValue)) {
            var parameterValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
            return adapters.Any(ad => string.Equals(ad.Name, parameterValueString, StringComparison.InvariantCultureIgnoreCase));
        }
        return false;
    }
}
