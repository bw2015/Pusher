using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pusher.Agent;
using Pusher.Caching;
using Pusher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pusher.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                foreach (PushMessage message in PushCaching.Instance().GetLog())
                {
                    PushAgent.Instance().SaveMessageLog(message);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
