using GSBase.ConfigCenter;
using GuardWebAPI.WeChat.Data;
using GuardWebAPI.WeChat.Middlewares;
using GuardWebAPI.WeChat.Services.JSSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardWebAPI.WeChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //ConfigCenterClient.Init();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddOptions();
            services.AddDbContext<GuardGoodsDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMemoryCache();
            services.AddMvc();

            services.AddTransient<JSSDKService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseMvcWithDefaultRoute();
            app.UseWebSockets().Map("/ws/goods", b => b.UseMiddleware<GuardGoodsAgentMiddleware>());
        }
    }
}