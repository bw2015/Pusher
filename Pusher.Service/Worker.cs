using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pusher.Agent;
using Pusher.Caching;
using Pusher.Models;
using SP.StudioCore.Utils;
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
                int count = 0, total = 0;
                foreach (PushMessage message in PushCaching.Instance().GetLog())
                {
                    count++;
                    total += message.Count;
                    PushAgent.Instance().SaveMessageLog(message);
                }
                if (count != 0)
                {
                    ConsoleHelper.WriteLine($"Save Message:{count} / Count:{total}", ConsoleColor.Green);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
