using System;

namespace CryoAOP.Core.Extensions
{
    public static class FileExtensions
    {
        public static string NormalisePath(this string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            return path.Replace("file:///", "").Replace("/", @"\");
        }
    }
}