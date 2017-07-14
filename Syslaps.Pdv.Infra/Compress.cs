using System.IO.Compression;

namespace Syslaps.Pdv.Infra
{
    public class Compress
    {
        public Compress()
        {

        }

        public static void ZipDirectory(string sourcePath, string destiny)
        {
           ZipFile.CreateFromDirectory(sourcePath, destiny);
        }
    }
}
