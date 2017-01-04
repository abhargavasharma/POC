using System.Linq;
using DapperExtensions;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Repository
{
    public interface IDbItemEncryptionService
    {
        T Decrypt<T>(T item) where T : DbItem;
        T Encrypt<T>(T item) where T : DbItem;
    }

    public class DbItemEncryptionService : IDbItemEncryptionService
    {
        private readonly IDapperExtensionsConfiguration _dapperExtensionsConfiguration;
        private readonly ISimpleEncryptionService _simpleEncryptionService;

        public DbItemEncryptionService(ISimpleEncryptionService simpleEncryptionService)
        {
            _simpleEncryptionService = simpleEncryptionService;
            _dapperExtensionsConfiguration = new DapperExtensionsConfiguration();
        }

        public T Decrypt<T>(T item) where T : DbItem
        {
            if (item != null && (!item.IsEncrypted.HasValue || item.IsEncrypted.Value))
            {
                var map = _dapperExtensionsConfiguration.GetMap<T>();
                var encryptedProperties = map.Properties.Where(p => p.Encrypted).Select(p => p.PropertyInfo);
                foreach (var encryptedProperty in encryptedProperties)
                {
                    var valueOfProperty = encryptedProperty.GetValue(item);
                    if (valueOfProperty != null)
                    {
                        var enryptedValue = encryptedProperty.GetValue(item).ToString();
                        var decryptedValue = _simpleEncryptionService.Decrypt(enryptedValue);
                        encryptedProperty.SetValue(item, decryptedValue);
                    }
                }
                item.IsEncrypted = false;
            }
            return item;
        }

        public T Encrypt<T>(T item) where T : DbItem
        {
            if (item != null && (!item.IsEncrypted.HasValue || !item.IsEncrypted.Value))
            {
                var map = _dapperExtensionsConfiguration.GetMap<T>();
                var encryptedProperties = map.Properties.Where(p => p.Encrypted).Select(p => p.PropertyInfo);
                foreach (var encryptedProperty in encryptedProperties)
                {
                    var valueOfProperty = encryptedProperty.GetValue(item);
                    if (valueOfProperty != null)
                    {
                        var decryptedValue = valueOfProperty.ToString();
                        var enryptedValue = _simpleEncryptionService.Encrypt(decryptedValue);
                        encryptedProperty.SetValue(item, enryptedValue);
                    }
                }
                item.IsEncrypted = true;
            }
            return item;
        }
    }
}
