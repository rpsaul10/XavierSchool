using System;
using System.Security.Cryptography;
using System.Text;

namespace XavierSchoolMicroService.Utilities
{
    public class EncryptionClass
    {
        public static string Encrypt(string str, string key)
        {
            try
            {
                TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

                byte[] hashKey = md5Provider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
                provider.Key = hashKey;
                provider.Mode = CipherMode.ECB;

                byte[] buff = ASCIIEncoding.ASCII.GetBytes(str);
                string strEncryp = Convert.ToBase64String(provider.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));

                md5Provider = null;
                
                return strEncryp;    
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static string Descrypt(string str, string key)
        {
            try
            {
                TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

                byte[] hashKey = md5Provider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
                provider.Key = hashKey;
                provider.Mode = CipherMode.ECB;

                byte[] buff = Convert.FromBase64String(str);
                string strDecryp = ASCIIEncoding.ASCII.GetString(provider.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));

                md5Provider = null;
                return strDecryp;    
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}