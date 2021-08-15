using Microsoft.Extensions.Hosting;
using SP.StudioCore.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Pusher.Services
{
    public class TimeService : IHostedService, IDisposable
    {
        private Timer timer;

        public void Dispose()
        {
            timer?.Dispose();
        }

        private void DoWork(object state)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            int count = PushService.Remove().Result;
            ConsoleHelper.WriteLine($"[TimeService - Remove] - {count} clients  -   {sw.ElapsedMilliseconds}ms", ConsoleColor.Cyan);
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            await Task.CompletedTask;
        }
    }
}
