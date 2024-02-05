using System.Text;
using System.Security.Cryptography;

namespace util
{
    class Aes128Crypto: IDisposable
    {
        readonly AesCryptoServiceProvider provider;
        bool disposed = false;

        public Aes128Crypto(string iv, string key)
        {
            provider = new AesCryptoServiceProvider();
            provider.IV = Encoding.UTF8.GetBytes(iv);
            provider.Key = Encoding.UTF8.GetBytes(key);
            provider.Mode = CipherMode.CBC;
            provider.Padding = PaddingMode.Zeros;
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            using (ICryptoTransform encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
            {
                return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
        }

        public byte[] Decrypt(byte[] encBytes)
        {
            using (ICryptoTransform decryptor = provider.CreateDecryptor(provider.Key, provider.IV))
            {
                return decryptor.TransformFinalBlock(encBytes, 0, encBytes.Length);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                provider.Dispose();
            }
            disposed = true;
        }
    }
}
