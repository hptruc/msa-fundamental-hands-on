using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MSA.Common.Contracts.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MSA.Common.Contracts.Domain;

namespace MSA.Common.MongoDB;

public static class Extensions 
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton(ServiceProvider => {
            var configuration = ServiceProvider.GetService<IConfiguration>();
            var serviceSetting = configuration?.GetSection(nameof(ServiceSetting))?.Get<ServiceSetting>();
            var mongoDBSetting = configuration?.GetSection(nameof(MongoDBSetting))?.Get<MongoDBSetting>();
            var mongoClient = new MongoClient(mongoDBSetting?.ConnectionString ?? throw new ArgumentException("MongoDB connection is null"));
            
            return mongoClient.GetDatabase(serviceSetting?.ServiceName ?? throw new ArgumentException("MongoDB service name is null"));
        });

        return services;
    }

    public static IServiceCollection AddRepositories<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider => 
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database ?? throw new ArgumentException("MongoDB service is null"), collectionName);
        });
        
        return services;
    }
}