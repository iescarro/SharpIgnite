using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SharpIgnite
{
    public class Crypt
    {
        public static string Encrypt(string text)
        {
            byte[] iv = Convert.FromBase64String(Config.Get("WeakEncryptionIV"));
            byte[] key = Encoding.ASCII.GetBytes(Config.Get("WeakEncryptionKey"));
            return new Cryptor(iv, key).Encrypt(text);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] iv = Convert.FromBase64String(Config.Get("WeakEncryptionIV"));
            byte[] key = Encoding.ASCII.GetBytes(Config.Get("WeakEncryptionKey"));
            return new Cryptor(iv, key).Decrypt(encryptedText);
        }

        public class Cryptor
        {
            byte[] iv;
            byte[] key;

            public Cryptor()
            {
                iv = Convert.FromBase64String(Config.Get("WeakEncryptionIV"));
                key = Encoding.ASCII.GetBytes(Config.Get("WeakEncryptionKey"));
            }

            public Cryptor(byte[] iv, byte[] key)
            {
                this.iv = iv;
                this.key = key;
            }

            public string Encrypt(string text)
            {
#warning This should only be used for obfuscation and should not be used to encrypt important information.
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.KeySize = 128;
                cipher.BlockSize = 128; //use 128 for compatibility with AES
                cipher.Padding = PaddingMode.PKCS7;
                cipher.Mode = CipherMode.CBC;
                cipher.IV = iv;
                cipher.Key = key;

                ICryptoTransform t = cipher.CreateEncryptor();
                byte[] textInBytes = Encoding.UTF8.GetBytes(text);
                byte[] result = t.TransformFinalBlock(textInBytes, 0, textInBytes.Length);
                return HttpUtility.UrlEncode(Convert.ToBase64String(result));
            }

            public string Decrypt(string encryptedText)
            {
#warning This should only be used for obfuscation and should not be used to encrypt important information.
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.KeySize = 128;
                cipher.BlockSize = 128; //use 128 for compatibility with AES
                cipher.Padding = PaddingMode.PKCS7;
                cipher.Mode = CipherMode.CBC;
                cipher.IV = iv;
                cipher.Key = key;

                ICryptoTransform t = cipher.CreateDecryptor();
                byte[] textInBytes = Convert.FromBase64String(encryptedText);
                byte[] result = t.TransformFinalBlock(textInBytes, 0, textInBytes.Length);
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
