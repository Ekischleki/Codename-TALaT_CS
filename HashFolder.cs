using System.Text;

namespace Codename_TALaT_CS
{
    public static class HashFolderUtils
    {
        public static string HashFolder(string path)
        {
            var hasher = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);
            byte[] input = Encoding.ASCII.GetBytes(GetFilesInFolder(path.ToLower()));
            hasher.BlockUpdate(input, 0, input.Length);
            byte[] result = new byte[64]; // 512 / 8 = 64
            hasher.DoFinal(result, 0);
            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();

        }

        public static string HashFolder(string[] paths)
        {
            var hasher = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);
            StringBuilder multibleFolder = new StringBuilder();
            foreach (var path in paths)
            {
                multibleFolder.Append(GetFilesInFolder(path.ToLower()));
            }
            Log.Shared.LogN("Hashing: " + multibleFolder.ToString());
            byte[] input = Encoding.ASCII.GetBytes(multibleFolder.ToString());
            hasher.BlockUpdate(input, 0, input.Length);
            byte[] result = new byte[64]; // 512 / 8 = 64
            hasher.DoFinal(result, 0);
            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }

        private static string GetFilesInFolder(string path)
        {
            string dirName = new DirectoryInfo(path).Name;
            StringBuilder result = new($"&{dirName}&");
            foreach (string file in Directory.GetFiles(path))
            {
                result.Append($"%{Path.GetFileName(file)}%");

                foreach (byte fileByte in File.ReadAllBytes(file))
                    result.Append($"{fileByte.ToString()};");
                result.Append("=");

            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                result.Append(GetFilesInFolder(dir));
            }
            result.Append("$");
            return result.ToString();
        }


    }

}