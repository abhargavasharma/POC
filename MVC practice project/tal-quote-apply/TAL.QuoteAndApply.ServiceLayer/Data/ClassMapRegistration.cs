using System.Reflection;
using TAL.QuoteAndApply.Infrastructure.Ioc;

namespace TAL.QuoteAndApply.ServiceLayer.Data
{
    public class ClassMapRegistration
    {
        private static bool _hasActivated;
        private static volatile object PadLock;

        static ClassMapRegistration() 
        {
            PadLock = new object();
        }

        public static void SafeRegisterAllClassMapsFromAssemblies(string nameStartsWith)
        {
            if (_hasActivated)
                return;

            lock (PadLock)
            {
                if (_hasActivated)
                    return;

                SubClassActivator.ActivateForAssembly<IDbItemClassMapper>(Assembly.GetCallingAssembly(), nameStartsWith);
                _hasActivated = true;
            }
        }

    }
}
