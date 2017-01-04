using System.IO;

namespace TAL.QuoteAndApply.Search.IntegrationTests.Service
{
    public class FileSystemHelper
    {
        public static void SetupAndCleanDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }
    }

}
