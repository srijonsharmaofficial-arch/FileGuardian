using System;
using System.IO;
using System.Security.Cryptography;
using FileGuardian.Interfaces;

namespace FileGuardian.Services
{
    public class HashService : IHashGenerator
    {
        public enum Algorithm { MD5, SHA1, SHA256 }

        private readonly Algorithm _algorithm;

        public HashService(Algorithm algorithm = Algorithm.SHA256)
        {
            _algorithm = algorithm;
        }

        public string ComputeHash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                HashAlgorithm hasher = _algorithm switch
                {
                    Algorithm.MD5 => MD5.Create(),
                    Algorithm.SHA1 => SHA1.Create(),
                    Algorithm.SHA256 => SHA256.Create(),
                    _ => throw new NotSupportedException("Unknown algorithm")
                };

                using (hasher)
                {
                    byte[] hashBytes = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}