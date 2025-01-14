using System.Collections.Generic;
using System.Linq;
using Jackett.Common.Models;
using Jackett.Common.Services.Interfaces;
using Jackett.Common.Utils.Logging;
using NLog;
using NLog.Targets;

namespace Jackett.Common.Services
{
    [Target("LogService")]
    public class LogCacheService : TargetWithLayout, ILogCacheService
    {
        private static List<CachedLog> _Logs = new List<CachedLog>();

        public List<CachedLog> Logs
        {
            get
            {
                lock (_Logs)
                {
                    return _Logs.ToList();
                }
            }
        }

        protected override void Write(LogEventInfo logEvent) => AddLog(logEvent);

        private static void AddLog(LogEventInfo logEvent)
        {
            lock (_Logs)
            {
                _Logs.Insert(0, new CachedLog
                {
                    Level = logEvent.Level.Name,
                    Message = CleanseLogMessage.Cleanse(logEvent.FormattedMessage),
                    When = logEvent.TimeStamp
                });

                _Logs = _Logs.Take(200).ToList();
            }
        }
    }
}
