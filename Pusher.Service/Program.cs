using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pusher.Service.Consumers;
using SP.StudioCore.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pusher.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConsumerStartup.Run<Startup>();
            Thread.Sleep(-1);
        }
    }
}
