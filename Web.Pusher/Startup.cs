using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SP.StudioCore.Ioc;
using SP.StudioCore.Log;
using SP.StudioCore.Services;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Pusher.Services;
using Web.Pusher.Middles;
using SP.StudioCore.Mvc.MiddleWare;

namespace Web.Pusher
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services
              .AddHostedService<TimeService>()
              //.AddSpLogging()
              //.AddSingleton(t => Setting.NewElasticClient())
              .AddSingleton(t => new IPHeader(new[] { "X-Forwarded-For" }))
              .AddCors(opt => opt.AddPolicy("Api", policy =>
              {
                  policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                  policy.AllowAnyHeader();
                  policy.AllowAnyOrigin();
                  policy.AllowAnyMethod();
              }))
              .Initialize()
              .AddService();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpContext()
                .UseWebSockets(new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(3)
                })
                .UseMiddleware<HeaderRouteMiddleware>()
                .UseMiddleware<WSMiddleware>()
                .UseStaticFiles()
                .UseRouting()
                .UseCors("Api")
                .UseAuthentication()
                .UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors("Api"); });
        }
    }
}
