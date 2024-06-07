using MSA.BankService.Domain;
using MSA.BankService.Infrastructure.Data;
using MSA.BankService.Services;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.Common.PostgresMassTransit.MassTransit;
using MSA.Common.Security.Authentication;
using MSA.Common.Security.Authorization;
using Microsoft.OpenApi.Models;
using MSA.Common.Contracts.Settings;
using Polly;
using Npgsql;
using MSA.BankService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services
    .AddPostgres<BankDbContext>()
    .AddPostgresRepositories<BankDbContext, Payment>()
    .AddPostgresUnitofWork<BankDbContext>()
    .AddMassTransitWithPostgresOutbox<BankDbContext>()
    .AddMSAAuthentication()
    .AddMSAAuthorization(opt =>
    {
        opt.AddPolicy("read_access", policy =>
        {
            policy.RequireClaim("scope", "paymentapi.read");
        });
        opt.AddPolicy("write_access", policy =>
        {
            policy.RequireClaim("scope", "paymentapi.write");
        });
    });

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddEndpointsApiExplorer();

ServiceUrlsSetting? srvUrlsSetting = builder.Configuration.GetSection(nameof(ServiceUrlsSetting))?.Get<ServiceUrlsSetting>();
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
                    { "paymentapi.read", "Access read operations" },
                    { "paymentapi.write", "Access write operations" }
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
        options.OAuthClientId("payment-swagger");
        options.OAuthScopes("profile", "openid");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var retryPolicy = Policy
    .Handle<NpgsqlException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

retryPolicy.ExecuteAndCapture(() => DbInitializer.InitDatabase(app));

app.Run();
