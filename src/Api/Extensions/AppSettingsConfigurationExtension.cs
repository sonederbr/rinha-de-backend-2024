namespace Api.Extensions
{
    public static class AppSettingsConfigurationExtension
    {
        public static WebApplicationBuilder AddAppSettingsEnvironment(this WebApplicationBuilder builder)
        {
            if (builder.Environment.ApplicationName == "testhost") builder.Environment.EnvironmentName = "Test";

            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            return builder;
        }
    }
}