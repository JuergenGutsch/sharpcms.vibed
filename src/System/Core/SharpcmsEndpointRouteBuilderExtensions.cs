using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sharpcms.Core
{
    public static class SharpcmsEndpointRouteBuilderExtensions
    {
        public static RouteHandlerBuilder MapSharpcms(this IEndpointRouteBuilder app)
        {
            return app.MapFallback(() => Results.Text(Core.Send(), "text/html"));
        }
    }
}
