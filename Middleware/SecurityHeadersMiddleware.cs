namespace SDTechnicalAssessment.Middleware
{
    // ------------------------------------------------------------
    // This middleware adds common HTTP security headers.
    //
    // These headers provide an additional layer of protection
    // for clients consuming our API.
    // ------------------------------------------------------------
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prevent browsers from guessing the response content type.
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Prevent the API response from being embedded in a frame.
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // Control how much referrer information is sent.
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            // Disable browser features that our API does not need.
            context.Response.Headers["Permissions-Policy"] =
                "camera=(), microphone=(), geolocation=()";

            // Continue processing the request.
            await _next(context);
        }
    }
}