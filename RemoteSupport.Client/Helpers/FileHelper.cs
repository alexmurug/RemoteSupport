using System.IO;

namespace RemoteSupport.Client.Helpers
{
    public static class FileHelper
    {
        public static byte[] SampleBytesFromFile(string filePath, int currentPosition, int bufferSize)
        {
            var length = bufferSize;
            var fs = new FileStream(filePath, FileMode.Open);
            fs.Position = currentPosition;

            if (currentPosition + length > fs.Length)
            {
                length = (int) (fs.Length - currentPosition);
            }

            var b = new byte[length];
            fs.Read(b, 0, length);
            fs.Dispose();
            return b;
        }

        public static long GetFileLength(string filePath)
        {
            var info = new FileInfo(filePath);
            return info.Length;
        }

        public static void AppendAllBytes(string filePath, byte[] bytes)
        {
            var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();
        }
    }
}