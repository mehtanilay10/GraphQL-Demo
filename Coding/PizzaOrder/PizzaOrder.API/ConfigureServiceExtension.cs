using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Server;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using PizzaOrder.Business.Helpers;
using PizzaOrder.Business.Services;
using PizzaOrder.Data;
using PizzaOrder.GraphQLModels;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Mutations;
using PizzaOrder.GraphQLModels.Queries;
using PizzaOrder.GraphQLModels.Schema;
using PizzaOrder.GraphQLModels.Subscription;
using PizzaOrder.GraphQLModels.Types;

namespace PizzaOrder.API
{
    public static class ConfigureServiceExtension
    {
        public static void AddCustomIdentityAuth(this IServiceCollection services)
        {
            // Added Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<PizzaDBContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password configs
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // ApplicationUser settings
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            });
        }

        public static void AddCustomJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Added JWT Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration.GetSection("JwtIssuerOptions:SecretKey").Value));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JwtIssuerOptions:Issuer").Value,
                    ValidAudience = configuration.GetSection("JwtIssuerOptions:Audience").Value,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void AddCustomService(this IServiceCollection services)
        {
            services.AddTransient<IPizzaDetailsService, PizzaDetailsService>();
            services.AddTransient<IOrderDetailsService, OrderDetailsService>();
            services.AddTransient<IEventService, EventService>();
        }

        public static void AddCustomGraphQLServices(this IServiceCollection services)
        {
            // GraphQL services
            services.AddScoped<IServiceProvider>(c => new FuncServiceProvider(type => c.GetRequiredService(type)));
            services.AddGraphQL(options =>
            {
                options.EnableMetrics = true;
                options.ExposeExceptions = true; // false prints message only, true will ToString
                options.UnhandledExceptionDelegate = context =>
                {
                    Console.WriteLine("Error: " + context.OriginalException.Message);
                };
            })
            .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User })
            .AddWebSockets()
            .AddDataLoader()
            .AddGraphTypes(typeof(PizzaOrderSchema));
        }

        public static void AddCustomGraphQLTypes(this IServiceCollection services)
        {
            services.AddSingleton<OrderDetailsType>();
            services.AddSingleton<PizzaDetailsType>();
            services.AddSingleton<EventDataType>();

            services.AddSingleton<OrderStatusEnumType>();
            services.AddSingleton<ToppingsEnumType>();
            services.AddSingleton<CompletedOrdersSortingFieldsEnumType>();
            services.AddSingleton<SortingDirectionEnumType>();

            services.AddSingleton<OrderDetailsInputType>();
            services.AddSingleton<PizzaDetailsInputType>();
            services.AddSingleton<CompletedOrderOrderByInputType>();

            services.AddSingleton<PizzaOrderQuery>();
            services.AddSingleton<PizzaOrderSchema>();
            services.AddSingleton<PizzaOrderMutation>();
            services.AddSingleton<PizzaOrderSubscription>();
        }

        public static void AddCustomGraphQLAuth(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.TryAddSingleton(_ =>
            {
                var authSettings = new AuthorizationSettings();

                authSettings.AddPolicy(
                    Constants.AuthPolicy.CustomerPolicy,
                    policy => policy.RequireClaim(ClaimTypes.Role, Constants.Roles.Customer));

                authSettings.AddPolicy(
                    Constants.AuthPolicy.RestaurantPolicy,
                    policy => policy.RequireClaim(ClaimTypes.Role, Constants.Roles.Restaurant));

                authSettings.AddPolicy(
                    Constants.AuthPolicy.AdminPolicy,
                    policy => policy.RequireClaim(ClaimTypes.Role, Constants.Roles.Customer, Constants.Roles.Restaurant, Constants.Roles.Admin));

                return authSettings;
            });
        }
    }
}
