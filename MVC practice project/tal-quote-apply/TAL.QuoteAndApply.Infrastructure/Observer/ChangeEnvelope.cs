using System;

namespace TAL.QuoteAndApply.Infrastructure.Observer
{

    public class ChangeEnvelope
    {
        private object Message { get; set; }

        public Type MessageType
        {
            get { return Message.GetType(); }
        }

        public T GetInstance<T>() where T : class
        {
            return Convert.ChangeType(Message, MessageType) as T;
        }

        public ChangeEnvelope(object message)
        {
            Message = message;
        }
    }
}
