using System;
using log4net;

namespace TAL.QuoteAndApply.Infrastructure.Logging
{
    public interface ILoggingService
    {
        void Error(Exception ex);
        void Error(string msg);
        void Error(string msg, Exception ex);
        void Info(string format, params object[] args);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILog _log;

        public LoggingService(ILog log)
        {
            _log = log;
        }

        public void Error(Exception ex)
        {
            _log.Error(ex.Message, ex);
        }

        public void Error(string msg)
        {
            _log.Error(msg);
        }

        public void Error(string msg, Exception ex)
        {
            _log.Error(msg, ex);
        }

        public void Info(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }
    }
}
