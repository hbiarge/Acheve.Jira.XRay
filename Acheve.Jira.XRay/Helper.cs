using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Acheve.Jira.XRay
{
    public static class Helper
    {
        public static string BuildBasicAuthentication(string userName, string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
        }

        public static string BuildUri(string prefix, string relative)
        {
            var path = prefix.EndsWith("/")
                ? prefix + relative
                : prefix + "/" + relative;

            return path;
        }

        public static void ExtractZipStream(Stream zipStream, string targetDirectory)
        {
            Console.WriteLine($"  Decompressing features in: {targetDirectory}");

            if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine($"  Directory does not exist. Creating: {targetDirectory}");
                Directory.CreateDirectory(targetDirectory);
            }

            using (ZipArchive zipFile = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                Console.WriteLine("  Features found:");

                foreach (ZipArchiveEntry zipEntry in zipFile.Entries)
                {
                    var path = Path.Combine(targetDirectory, zipEntry.FullName);
                    Console.WriteLine($"    {path}");
                    zipEntry.ExtractToFile(path, true);
                }
            }
        }
    }
}