namespace Rally.ServiceLayer.Rally
{
    public interface IRallyApiKeyProvider
    {
        string GetToken();
    }

    public class RallyApiKeyProvider : IRallyApiKeyProvider
    {
        public string GetToken()
        {
            return "_LXGaFvTWR4asYpl1Lb5D42c8dOcs1k4zgP9sHWRDIqw"; //TODO: replace this with config value
        }
    }
}