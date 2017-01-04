namespace TAL.QuoteAndApply.Infrastructure.Crypto
{
    public interface ISecurityService
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }

    public class SecurityService : ISecurityService
    {
        //TODO: Encryption password from config file and change (this is currently CUE's one)
        private const string EncryptionPassword =
            "8zcVT8dljd4KqyPUzeRLR+SbvDYcvZf/s5m/yNT1MxhPaEux7kpWvUXiAjYP8U9CvbXwHKqFbnBJh6T+2BDF1xSd54/ngNmrFaNn4Sy1rYRbG9SOCYm8RGOYm60oFfufiJi13X9pL5hnOrx6SEHIqu6xUulU/vq84XFTpTsC+SgBQ1kHhPRl6/UBx///PhTdNrUI4zFrpXBsFsmdJsAWAS2yEnqD4434p4pSdADmFBj9bNN3dRA8UVfIIcvGiUgwgNs9BwSa11+7GMUasbHYjIV/IF//+EIdteVk/2zCol+3v3cQqV5NZcWfWZ+LHkOZhhiBdEdsg+3GInXlFQeTD168dLjsE3eBxeYwrKNZsU8GWV8caxyR2H487XiclmO2T9YVrRnAgBEVaKuybiKKI3IYQq8pmI7RNNMm2YPFu4elRqUqXgUT3tPr3/lEqukmXfVOn4L4t3GbzB8MXAO41gqRj29XnbhzERAC3L+geFz6VIkF7EWtudgvm0xRKCePY29JczwS6WzrCfW538TohmYaFHrJcvhj/jSjElV53W4+dEtLwiI2JvTw1pLA5aaTJw8/Zq7BMS+zGboSH5Ne/F08bY+wr+KQjv9jeD0XaaEla4m9yBbTzG5RKZkgPnTQ5GaWTgTcOWleJAmJSgxxju5FklgX6G0O+9bSjTlhPEZeskITaspmhLohFfLZ/i3mPVf/4K05Io66Y6x3vkhvewCymhRvwt7F3CjQ4XJd3OHAn2vYBh/ZAoyTFRkYlRpNJ3rZTUcolEvTDGk5C6to2g0iGHc7cfA9RpDaTdPHTfM9MrLDcytZn3JadLiwvjC7nLJAFtBzEOzAx2yn7DaQf3WuSMC2rfKyFxa0LTpodNenqQSzFl8bu2YGYNGuQB8g27L5xMFHmTbMGBnnF0SH/TKjTcX1UMTJ+jMDnJlhlRw=";

        public string Encrypt(string value)
        {
            return Crypto.Encrypt(value, EncryptionPassword);
        }

        public string Decrypt(string value)
        {
            return Crypto.Decrypt(value, EncryptionPassword);
        }
    }
}
