namespace MessAidVOne.API.Extensions
{
    public static class MiddlewareExtensions 
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices.GetService<IServiceCollection>();
            if (services != null)
            {
                services.AddScoped<GlobalExceptionMiddleware>();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
