using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace LSA.OrderFlow.CrossCutting;

public class MongoModule : Module
{
    protected override void Load(ContainerBuilder b)
    {
        b.Register(ctx =>
        {
            var cfg = ctx.Resolve<IConfiguration>();
            return new MongoClient(cfg.GetConnectionString("Mongo"));
        }).As<IMongoClient>().SingleInstance();

        b.Register(ctx =>
        {
            var cfg = ctx.Resolve<IConfiguration>();
            var client = ctx.Resolve<IMongoClient>();
            return client.GetDatabase(cfg["MongoDatabase"]);
        }).As<IMongoDatabase>().SingleInstance();
    }
}