using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpz
{
    public static class TempFileHelpz
    {
        public static bool Exists(string filename)
        {
            return File.Exists(Path.Combine(Path.GetTempPath(), filename));
        }

        public static string GetFilePath(string filename)
        {
            return Path.Combine(Path.GetTempPath(), filename);
        }

        public static string MakeFilePath(string extension)
        {
            return Path.ChangeExtension(Path.GetTempFileName(), extension);
        }
    }
}
