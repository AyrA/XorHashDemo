using System.Security.Cryptography;
using System.Text;

namespace XorHashDemo
{
    internal static class HashTools
    {
        /// <summary>
        /// Hash all individual files in a directory and create a xorsum of the hashes
        /// </summary>
        /// <param name="dir">Directory</param>
        /// <returns>xorsum value</returns>
        public static byte[] HashDir(string dir)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dir);
            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException(dir);
            }
            dir = Path.GetFullPath(dir);
            byte[] xor = new byte[SHA256.HashSizeInBytes];
            //This is just a demo and will not do recursion as of now
            foreach (var fn in Directory.EnumerateFiles(dir))
            {
                //This is the filename minus the root path.
                //Since we don't do recursion yet, this is currently identical to "Path.GetFileName(fn)"
                var metadata = fn[(dir.Length + 1)..];
                using var fs = File.OpenRead(fn);
                byte[] fileHash = HashFile(fs, metadata);
                Console.WriteLine("{0}\t{1}", metadata, ToHex(fileHash));
                XorBytes(fileHash, xor);
            }
            return xor;
        }

        /// <summary>
        /// Converts sequence of bytes into an uppercase hex string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Hex string</returns>
        public static string ToHex(IEnumerable<byte> data)
        {
            ArgumentNullException.ThrowIfNull(data);
            return string.Concat(data.Select(m => m.ToString("X2")));
        }

        /// <summary>
        /// XOR two byte arrays together. This updates <paramref name="existingBuffer"/>
        /// </summary>
        /// <param name="newData">New data to add to the xorsum</param>
        /// <param name="existingBuffer">Existing xorsum value</param>
        public static void XorBytes(byte[] newData, byte[] existingBuffer)
        {
            ArgumentNullException.ThrowIfNull(newData);
            ArgumentNullException.ThrowIfNull(existingBuffer);
            ArgumentOutOfRangeException.ThrowIfNotEqual(newData.Length, existingBuffer.Length);

            for (var i = 0; i < existingBuffer.Length; i++)
            {
                existingBuffer[i] ^= newData[i];
            }
        }

        /// <summary>
        /// Computes the SHA256 of a file with metadata
        /// </summary>
        /// <param name="fileData">Stream containing data to hash</param>
        /// <param name="metadata">Metadata to hash
        /// (see comment in <see cref="HashDir(string)"/> on how this is built)
        /// </param>
        /// <returns>File hash</returns>
        public static byte[] HashFile(Stream fileData, string metadata)
        {
            using var Hasher = SHA256.Create();
            var metadataBytes = Encoding.UTF8.GetBytes(metadata + "\0");
            using var CS = new CryptoStream(Stream.Null, Hasher, CryptoStreamMode.Write);
            CS.Write(metadataBytes, 0, metadataBytes.Length);
            fileData.CopyTo(CS);
            CS.FlushFinalBlock();
            return Hasher.Hash ?? throw new Exception("Hash not computed");
        }
    }
}
