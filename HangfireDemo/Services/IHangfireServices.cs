using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Services
{
    public interface IHangfireServices
    {
        public void TaskOne();

        public void TaskTwo();
    }

    public class HangfireServices: IHangfireServices
    {
        private readonly ILogger<HangfireServices> _logger;
        public HangfireServices(ILogger<HangfireServices> logger)
        {
            _logger = logger;
        }

        public void TaskOne()
        {
            _logger.LogDebug("十分钟执行一次");
        }

        public void TaskTwo()
        {
            _logger.LogDebug("一分钟执行一次");
        }
    }
}
