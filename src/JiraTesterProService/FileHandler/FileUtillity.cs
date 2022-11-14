using System.Reflection;

namespace JiraTesterProService.FileHandler;

public static class FileUtillity
{
    public static string AssemblyDirectory

    {

        get

        {


            string codeBase = Assembly.GetExecutingAssembly().Location;

            UriBuilder uri = new UriBuilder(codeBase);

            string path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);

        }

    }
}