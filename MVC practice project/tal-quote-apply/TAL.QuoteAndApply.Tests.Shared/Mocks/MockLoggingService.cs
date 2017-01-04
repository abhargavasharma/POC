using System;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Tests.Shared.Mocks
{
    public class MockLoggingService : ILoggingService
    {
        /*
            Currently doesn't do anything but if we wanted to test that certain messages were logged, this class could become more fancier
        */

        public void Error(Exception ex)
        {
            //I don't do anything at the moment
            Console.WriteLine(ex);
        }

        public void Error(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Error(string msg, Exception ex)
        {
            //I don't do anything at the moment
            Console.WriteLine(msg);
            Console.WriteLine(ex);
        }

        public void Info(string format, params object[] args)
        {
            //I don't do anything at the moment
        }
    }
}
