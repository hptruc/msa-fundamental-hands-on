using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;
using MSA.Common.Contracts.Settings;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Services;
using MSA.Common.PostgresMassTransit.MassTransit;
using Polly;
using Npgsql;
using MSA.Common.Security.Authentication;
using MSA.Common.Security.Authorization;
using Microsoft.OpenApi.Models;
using MSA.OrderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var serviceSetting = builder.Configuration.GetSection(nameof(PostgresDBSetting))?.Get<PostgresDBSetting>();
var srvUrlsSetting = builder.Configuration.GetSection(nameof(ServiceUrlsSetting))?.Get<ServiceUrlsSetting>();

// Add services to the container.
builder.Services
    .AddPostgres<MainDbContext>()
    .AddPostgresRepositories<MainDbContext, Order>()
    .AddPostgresRepositories<MainDbContext, Product>()
    .AddPostgresUnitofWork<MainDbContext>()
    .AddMassTransitWithPostgresOutbox<MainDbContext>()
    .AddMSAAuthentication()
    .AddMSAAuthorization(opt =>
    {
        opt.AddPolicy("read_access", policy =>
        {
            policy.RequireClaim("scope", "orderapi.read");
        });
        opt.AddPolicy("write_access", policy =>
        {
            policy.RequireClaim("scope", ["orderapi.write", "paymentapi.write", "productapi.read"]);
        });
    });

builder.Services.AddHttpClient<IProductService, ProductService>(cfg =>
{
    cfg.BaseAddress = new Uri(srvUrlsSetting?.ProductServiceUrl ?? throw new ArgumentException("ProductServiceUrl is null"));
});

builder.Services.AddHttpClient<IPaymentService, PaymentService>(cfg =>
{
    cfg.BaseAddress = new Uri(srvUrlsSetting?.BankServiceUrl ?? throw new ArgumentException("BankServiceUrl is null")); 
});

builder.Services.AddControllers(opt =>
{
    opt.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var scheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{srvUrlsSetting?.IdentityServiceUrl ?? throw new ArgumentException("Identity config is null")}/connect/authorize"),
                TokenUrl = new Uri($"{srvUrlsSetting.IdentityServiceUrl}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "orderapi.read", "Access read operations" },
                    { "orderapi.write", "Access write operations" },
                    { "paymentapi.write", "Payment access write operations" },
                    { "productapi.read", "Product access write operations" }
                }
            }
        },
        Type = SecuritySchemeType.OAuth2
    };

    options.AddSecurityDefinition("OAuth", scheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme }
            },
            new List<string> { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("order-swagger");
        options.OAuthScopes("profile", "openid");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var retryPolicy = Policy
    .Handle<NpgsqlException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

retryPolicy.ExecuteAndCapture(() => DbInitializer.InitDatabase(app));

app.Run();