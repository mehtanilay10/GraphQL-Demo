using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PizzaOrder.Data;
using PizzaOrder.GraphQLModels.Schema;

namespace PizzaOrder.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // For Async IO Operations
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers();

            services.AddDbContext<PizzaDBContext>(
                optionsAction: options => options.UseSqlServer(Configuration["ConnectionStrings:PizzaOrderDB"]),
                contextLifetime: ServiceLifetime.Singleton);

            services.AddCustomIdentityAuth();

            services.AddCustomJWT(Configuration);

            services.AddCustomGraphQLAuth();

            services.AddCustomService();

            services.AddCustomGraphQLServices();

            services.AddCustomGraphQLTypes();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PizzaDBContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbContext.EnsureDataSeeding();

            app.UseWebSockets();

            app.UseGraphQL<PizzaOrderSchema>();

            app.UseGraphQLWebSockets<PizzaOrderSchema>();

            app.UseGraphQLPlayground();
        }
    }
}
