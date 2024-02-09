namespace Api.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddMySwagger(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Rinha de Backend - 2024/Q1",
                    Description = "A Rinha de Backend é um desafio que tem como principal objetivo compartilhar conhecimento em formato de desafio!",
                    TermsOfService = new Uri("https://github.com/zanfranceschi/rinha-de-backend-2024-q1"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = new Uri("https://twitter.com/rinhadebackend")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://github.com/zanfranceschi/rinha-de-backend-2024-q1")
                    }
                });

                // Generate summary documentation using reflection
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            return services;
        }

        public static void UseMySwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/swagger/v1/swagger.json", "My Project API");
            });
        }
    }
}
