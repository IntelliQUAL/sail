using System;
using System.Text;
using System.Security.Cryptography;

namespace SAIL.Framework.Host.Bootstrap
{
    public class DefaultMD5Handler : IMD5
    {
        string IMD5.ComputeHash(string input)
        {
            string encryptedSignature = string.Empty;

            // Convert the input into byte array
            byte[] signature = Encoding.Default.GetBytes(input.ToString());

            // Create the MD5 provider
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            //  Hash the byte array                        
            signature = provider.ComputeHash(signature);

            // Conver the byte array hash back into a string.
            encryptedSignature = Convert.ToBase64String(signature);

            return encryptedSignature;
        }
    }
}
