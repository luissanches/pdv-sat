using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Syslaps.Pdv.Infra.SAT
{
    public static class SatUtils
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        private static string GetDllFolderPath(string providerName)
        {
            if (Environment.Is64BitOperatingSystem)
                return string.Format("SatReferences\\{0}32", (object)providerName);

            return string.Format("SatReferences\\{0}32", (object)providerName);
        }

        public static void SetDllFolderPath(string providerName)
        {
            SatUtils.SetDllDirectory(SatUtils.GetDllFolderPath(providerName));
        }

        public static List<string> ListReferenceFoldersPath()
        {
            return new List<string>() { "SatReferences\\sweda32", "SatReferences\\bematech32", "SatReferences\\elgin32", "SatReferences\\gertec32", "SatReferences\\urano32", "SatReferences\\kryptus32", "SatReferences\\dimep32", "SatReferences\\tanca32" };
        }

        public static void CopyReferencesToRoot(string providerName)
        {
            foreach (FileInfo file in new DirectoryInfo("SatReferences\\" + providerName.ToLower() + "32").GetFiles())
            {
                FileInfo fileInfo = new FileInfo(Path.Combine("", file.Name));
                if (!fileInfo.Exists || file.Length != fileInfo.Length || !(file.LastWriteTime == fileInfo.LastWriteTime))
                {
                    try
                    {
                        file.CopyTo(fileInfo.FullName, true);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Erro ao copiar arquivo: \"{(object) file.FullName}\" para \"{(object) fileInfo.FullName}\" -- Erro: {(object) ex.Message}", ex);
                    }
                }
            }
        }

        
    }
}
