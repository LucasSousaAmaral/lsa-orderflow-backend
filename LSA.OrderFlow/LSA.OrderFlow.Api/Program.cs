using FluentValidation;
using LSA.OrderFlow.Application.Common.Validators;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.Commands.CreateOrder;
using LSA.OrderFlow.Infrastructure.Mongo.Projections;
using LSA.OrderFlow.Infrastructure.Mongo.Repositories;
using LSA.OrderFlow.Infrastructure.Sql;
using LSA.OrderFlow.Infrastructure.Sql.Seeding;
using LSA.OrderFlow.Infrastructure.Sql.Outbox;
using LSA.OrderFlow.Infrastructure.Sql.Repositories;
using LSA.OrderFlow.Api.HostedServices;
using LSA.OrderFlow.Api.Health;
using LSA.OrderFlow.Api.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HealthChecks
builder.Services.AddHealthChecks()
	.AddCheck<SqlHealthCheck>("sql")
	.AddCheck<MongoHealthCheck>("mongo");

// EFCore SQL
builder.Services.AddDbContext<OrderFlowDbContext>(opt =>
	opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// Mongo
builder.Services.AddSingleton<IMongoClient>(_ =>
	new MongoClient(builder.Configuration.GetConnectionString("Mongo")));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
	sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["MongoDatabase"]));

// Mongo Config
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));

// MediatR
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));

// Commands
builder.Services.AddScoped<IOrderRepository, OrderRepositorySql>();
builder.Services.AddScoped<IProductRepository, ProductRepositorySql>();
builder.Services.AddScoped<LSA.OrderFlow.Application.Abstractions.IUnitOfWork, UnitOfWorkSql>();
builder.Services.AddScoped<LSA.OrderFlow.Application.Abstractions.IOutbox, OutboxSql>();

// Queries
builder.Services.AddScoped<IOrderReadRepository, OrderReadRepositoryMongo>();
builder.Services.AddScoped<IProjectionDispatcher, ProjectionDispatcher>();

// Seeds
builder.Services.AddScoped<OrderFlowSeeder>();
builder.Services.AddHostedService<DatabaseSeedHostedService>();

// Background Outbox Processor
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.MapHealthChecks("/health");
app.MapControllers();
app.Run();