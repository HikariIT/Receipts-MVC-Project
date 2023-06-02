using MVCProject.Utility;

namespace MVCProject.Middlewares; 

public class AuthMiddleware {
    
    private readonly RequestDelegate _next;
    
    public AuthMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        // Authentication is not required for API endpoints, Home and Auth (Auth is login and register)
        if (context.Request.Path.StartsWithSegments("/api") || 
            context.Request.Path.StartsWithSegments("/auth") || 
            context.Request.Path.Value == "/") {
            Console.WriteLine("Skipping auth middleware for " + context.Request.Path);
            await _next(context);
            return;
        }

        Console.WriteLine(context.Session.GetString("user") + " is accessing " + context.Request.Path + "");

        // If the user is not logged in, redirect to login page
        if (context.Session.GetString("user") == null) {
            context.Response.Redirect("/auth/login");
            return;
        }

        if (!JwtManager.IsValid(context.Session.GetString("user")!)) {
            context.Response.Redirect("/auth/login");
            context.Session.Clear();
            Console.WriteLine("Invalid signature");
            return;
        }
        
        if (context.Request.Path.StartsWithSegments("/home/admin")) {
            Console.WriteLine("User is accessing admin page: " + context.Request.Path + "");
            var payload = JwtManager.ExtractPayload(context.Session.GetString("user"));
            if (payload.user != "1") {
                context.Response.Redirect("/home/dashboard");
                return;
            }
        }
        
        await _next(context);
    }
}

public static class AuthMiddlewareExtensions {
    public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder) {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}