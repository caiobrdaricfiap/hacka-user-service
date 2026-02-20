namespace user_service.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Usuario Service",
                    Version = "v1",
                    Description = "Microserviço responsável por criação e autenticação de usuários",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Microserviço User",
                        Email = "Temp@Temp.com"
                    }
                });
            });
        }
    }
}
