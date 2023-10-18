using System.Security.Cryptography;
using System.Text;

namespace FolderSync
{
    internal class FileCompare : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo? x, FileInfo? y)
        {

            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Both null is handled before
            if (x == null || y == null)
            {
                return false;
            }

            if (x.FullName == y.FullName)
            {
                return true;
            }

            // At last, compare the MD5 of the files.
            var hashX = GetHash(x.FullName);
            var hashY = GetHash(y.FullName);

            return hashX == hashY;
        }

        //the interface needs to implement this
        public int GetHashCode(FileInfo obj)
        {
            return obj.GetHashCode();
        }

        private static string GetHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
