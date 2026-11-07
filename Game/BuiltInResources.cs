using System;
using System.IO;

namespace WindBot.Game
{
    internal static class BuiltInResources
    {
        internal static Stream Open(string directory, string name, string extension, bool external)
        {
            if (external)
            {
                try
                {
                    return Program.ReadFile(directory, name, extension);
                }
                catch (FileNotFoundException)
                {
                }
                catch (DirectoryNotFoundException)
                {
                }
            }

            Stream stream = typeof(BuiltInResources).Assembly.GetManifestResourceStream(
                "WindBot." + directory + "." + name + "." + extension);
            if (stream == null)
                throw new FileNotFoundException("Built-in resource not found: " + directory + "/" + name, name);
            return stream;
        }
    }
}
