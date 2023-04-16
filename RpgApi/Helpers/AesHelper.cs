using System.Security.Cryptography;

namespace Rpg.Helpers
{
    // https://www.siakabaro.com/how-to-create-aes-encryption-256-bit-key-in-c/

    public static class AesHelper
    {
        public static byte[] GenerateKeyBytes()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static string GenerateKeyString()
        {
            var keyBytes = GenerateKeyBytes();
            return Convert.ToBase64String(keyBytes);
        }

        public static byte[] GenerateIvBytes(byte[] keyBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();
                return aes.IV;
            }
        }

        public static string GenerateIvString(string keyStr)
        {
            var keyBytes = Convert.FromBase64String(keyStr);
            var ivBytes = GenerateIvBytes(keyBytes);
            return Convert.ToBase64String(ivBytes);
        }

        public static string EncryptString(string plainStr, byte[] keyBytes, byte[] ivBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                var encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainStr);
                        }
                        var encryptedBytes = ms.ToArray();
                        var encryptedStr = Convert.ToBase64String(encryptedBytes);
                        return encryptedStr;
                    }
                }
            }
        }

        public static string DecryptString(string encryptedStr, byte[] keyBytes, byte[] ivBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                var decryptor = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            var decryptedStr = sr.ReadToEnd();
                            return decryptedStr;
                        }
                    }
                }
            }
        }
    }
}
