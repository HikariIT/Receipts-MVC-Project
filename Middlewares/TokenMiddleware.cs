using MVCProject.Database.Contexts;
using MVCProject.Utility;

namespace MVCProject.Middlewares; 

public class TokenMiddleware {
    private readonly RequestDelegate _next;
    private const string HEADER_KEY = "X-Api-Key";
    private const string HEADER_USER = "X-Api-User";
    private const bool DEBUG = true;
    
    public TokenMiddleware(RequestDelegate next) {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, ReceiptDatabaseContext db) {
        
        // Skip token 
        if (!context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/api/auth")) {
            await _next(context);
            return;
        }
        
        _log("Authenticating token: " + context.Request.Path);
        
        if (!context.Request.Headers.TryGetValue(HEADER_KEY, out var extractedApiKey)) {
            context.Response.StatusCode = 401;
            _log("API key not found");
            await context.Response.WriteAsync("API key not found");
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(HEADER_USER, out var extractedUserId)) {
            context.Response.StatusCode = 401;
            _log("User ID not found");
            await context.Response.WriteAsync("User ID not found");
            return;
        }
        
        _log("API key found: " + extractedApiKey + " for user " + extractedUserId + ". Verifying...");
        
        var user = db.Users.FirstOrDefault(u => u.user_id == int.Parse(extractedUserId));
        
        if (user == null) {
            context.Response.StatusCode = 401;
            _log("User not found");
            await context.Response.WriteAsync("User not found");
            return;
        }
        
        if (!user.api_token.Equals(extractedApiKey)) {
            context.Response.StatusCode = 401;
            _log("Unauthorized client");
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }

        await _next(context);
    }
    
    private void _log(object obj) {
        if (DEBUG)
            Console.WriteLine(obj.ToString());
    }
}

public static class TokenMiddlewareExtensions {
    public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware<TokenMiddleware>();
    }
}