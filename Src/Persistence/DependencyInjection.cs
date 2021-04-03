namespace Isitar.FileStorage.Persistence
{
    using Application.Interfaces;
    using FileDb;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var mds = new MongoDbSettings();
            configuration.Bind(nameof(MongoDbSettings), mds);
            services.AddSingleton(mds);

            services.AddTransient<IFileDb, MongoFileDb>();
            return services;
        }
    }
}