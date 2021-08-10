using Microsoft.Extensions.DependencyInjection;
using SP.StudioCore.Ioc;
using SP.StudioCore.Log;
using SP.StudioCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusher.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
              .AddSpLogging()
              //.AddSingleton(t => Setting.NewElasticClient())
              .Initialize()
              .AddService();
        }
    }
}
