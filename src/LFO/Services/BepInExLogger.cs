using BepInEx.Logging;
using LFO.Shared;

namespace LFO
{
    public class BepInExLogger : ILogger
    {
        private readonly ManualLogSource _logger;

        public BepInExLogger(ManualLogSource logger)
        {
            _logger = logger;
        }

        public void LogInfo(object message)
        {
            _logger.LogInfo(message);
        }

        public void LogDebug(object message)
        {
            _logger.LogDebug(message);
        }

        public void LogWarning(object message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(object message)
        {
            _logger.LogError(message);
        }
    }
}