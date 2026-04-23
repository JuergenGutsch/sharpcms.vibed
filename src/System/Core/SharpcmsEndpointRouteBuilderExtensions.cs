using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sharpcms.Core
{
    /// <summary>
    /// Provides endpoint routing extensions for mapping sharpcms responses.
    /// </summary>
    public static class SharpcmsEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps the sharpcms fallback endpoint.
        /// </summary>
        /// <param name="builder">The endpoint route builder.</param>
        /// <returns>The mapped fallback route handler.</returns>
        public static RouteHandlerBuilder MapSharpcms(this IEndpointRouteBuilder builder)
        {
            return builder.MapFallback(() => Results.Text(Core.Send(), "text/html"));
        }
    }
}
