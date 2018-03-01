/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Cryptography.cs
 *  Namespace:  Ewav.BAL    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    26/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ewav.BAL
{
    /// <summary>
    /// This class contains two static methods one to entrypt string and one to decrypt string
    /// </summary>
    public class  xCryptography
    {

        public string KeyForUserPasswordSalt = ConfigurationManager.AppSettings["KeyForUserPasswordSalt"];
        public string KeyForConnectionStringPassphrase = ConfigurationManager.AppSettings["KeyForConnectionStringPassphrase"];
        public string KeyForConnectionStringSalt = ConfigurationManager.AppSettings["KeyForConnectionStringSalt"];
        public string KeyForConnectionStringVector = ConfigurationManager.AppSettings["KeyForConnectionStringVector"];

        /// <summary>
        /// Identifier for Web driver that is built into Epi Info
        /// </summary>
     
        readonly string initVector;    

        readonly string passPhrase;
        readonly string saltValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cryptography" /> class.
        /// </summary>
        public  xCryptography()
        {
            passPhrase = KeyForConnectionStringPassphrase;  //   "80787d6053694493be171dd712e51c61";
            saltValue = KeyForConnectionStringSalt;  //   "476ba16073764022bc7f262c6d67ebef";
            initVector = KeyForConnectionStringVector;  //     "0f8f*d5bd&cb4~9f";
        }

        /// <summary>
        /// Decryption
        /// </summary>
        /// <param name="cipherText">The ciphertext to decrypt</param>
        /// <returns>The plaintext</returns>
        public string Decrypt(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            byte[] keyBytes = password.GetBytes(16);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }

        /// <summary>
        /// Encryption
        /// </summary>
        /// <param name="plainText">The plaintext to encrypt</param>
        /// <returns>The ciphertext</returns>
        public string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            byte[] keyBytes = password.GetBytes(16);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }
    }
}