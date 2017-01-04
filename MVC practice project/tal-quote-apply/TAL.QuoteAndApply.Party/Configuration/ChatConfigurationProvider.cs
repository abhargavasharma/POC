using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Party.Configuration
{
    public interface IChatConfigurationProvider
    {
        string WebChatUrl { get; }
        string StartTime { get; }
        string EndTime { get; }
    }

    public class ChatConfigurationProvider : IChatConfigurationProvider
    {
        public string WebChatUrl => ConfigurationManager.AppSettings["Chat.WebChatUrl"];
        public string StartTime => ConfigurationManager.AppSettings["Chat.StartHourWebChatAvailable"];
        public string EndTime => ConfigurationManager.AppSettings["Chat.EndHourWebChatAvailable"];
    }
}
