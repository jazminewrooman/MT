using System.Text;
using PCLCrypto;

    /// <summary>
    /// Common cryptographic helper
    /// </summary>
    public static class Crypto
    {
        public static byte[] String2ByteArray(string data)
        {
            byte[] retval;
            retval = new byte[data.Length / 2];

            for (int i = 0; i < data.Length / 2; i++)
            {
                int decValue = int.Parse(data.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                retval[i] = System.Convert.ToByte(decValue);
            }
            return retval;
        }

        /// <summary>
        /// Creates Salt with given length in bytes.
        /// </summary>
        /// <param name="lengthInBytes">No. of bytes</param>
        /// <returns></returns>
        public static byte[] CreateSalt(int lengthInBytes)
        {
            return WinRTCrypto.CryptographicBuffer.GenerateRandom(lengthInBytes);
        }

        /// <summary>
        /// Creates a derived key from a comnination 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="keyLengthInBytes"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static byte[] CreateDerivedKey(string password, byte[] salt, int keyLengthInBytes = 32, int iterations = 1000)
        {
            byte[] key = NetFxCrypto.DeriveBytes.GetBytes(password, salt, iterations, keyLengthInBytes);
            return key;
        }

        /// <summary>
        /// Encrypts given data using symmetric algorithm AES
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] EncryptAes(string data, string password, byte[] salt)
        {
            byte[] key = CreateDerivedKey(password, salt);

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, Encoding.UTF8.GetBytes(data));
            return bytes;
        }
        /// <summary>
        /// Decrypts given bytes using symmetric alogrithm AES
        /// </summary>
        /// <param name="data">data to decrypt</param>
        /// <param name="password">Password used for encryption</param>
        /// <param name="salt">Salt used for encryption</param>
        /// <returns></returns>
        public static string DecryptAes(byte[] data, string password, byte[] salt)
        {
            byte[] key = CreateDerivedKey(password, salt);

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

    }
