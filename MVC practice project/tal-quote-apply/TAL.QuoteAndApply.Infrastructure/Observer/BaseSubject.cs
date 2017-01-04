using System.Collections.Generic;

namespace TAL.QuoteAndApply.Infrastructure.Observer
{
    public interface ISubject<in TObserver>
    {
        void Subscribe(TObserver observer);
        void Notify(ChangeEnvelope changeEnvelope);
    }

    public abstract class BaseSubject<TObserver, TChange> : ISubject<TObserver> where TObserver : ISimpleObserver<TChange> where TChange : class
    {
        protected readonly IList<TObserver> Observers = new List<TObserver>();

        public void Subscribe(TObserver observer)
        {
            Observers.Add(observer);
        }

        public void Unsubscribe(TObserver observer)
        {
            Observers.Remove(observer);
        }

        public void Notify(ChangeEnvelope changeEnvelope)
        {
            foreach (var o in Observers)
            {
                o.Update(changeEnvelope.GetInstance<TChange>());
            }
        }
    }
}
