namespace Isitar.FileStorage.Application
{
    using System.Reflection;
    using Contract;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Settings;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var fsc = new FileStorageSettings();
            configuration.Bind(nameof(FileStorageSettings), fsc);
            services.AddSingleton(fsc);
            
            var ss = new SizeSettings();
            configuration.Bind(nameof(SizeSettings), ss);
            configuration.Bind(nameof(SizeSettings)+":Settings", ss.Settings);
            services.AddSingleton(ss);
            
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IFileStorageProvider, FileStorageProvider>();
            return services;
        }
    }
}