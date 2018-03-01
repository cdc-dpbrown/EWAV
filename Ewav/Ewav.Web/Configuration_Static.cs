/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Configuration_Static.cs
 *  Namespace:  Epi    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;

namespace Epi
{
    /// <summary>
    /// Class Configuration
    /// </summary>
    public partial class Configuration
    {


        /// <summary>
        /// Event Handler for ConfigurationUpdated
        /// </summary>
        public static event EventHandler ConfigurationUpdated;


        /// <summary>
        /// Encryption
        /// </summary>
        /// <param name="plainText">The plaintext to encrypt</param>
        /// <returns>The ciphertext</returns>
        public static string Encrypt(string plainText)
        {
            //byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            //byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            //byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            //PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            //byte[] keyBytes = password.GetBytes(16);
            //RijndaelManaged symmetricKey = new RijndaelManaged();
            //symmetricKey.Mode = CipherMode.CBC;
            //ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            //MemoryStream memoryStream = new MemoryStream();
            //CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            //cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            //cryptoStream.FlushFinalBlock();
            //byte[] cipherTextBytes = memoryStream.ToArray();
            //memoryStream.Close();
            //cryptoStream.Close();
            //string cipherText = Convert.ToBase64String(cipherTextBytes);
            return "Encrypt";
        }

        /// <summary>
        /// Decryption
        /// </summary>
        /// <param name="cipherText">The ciphertext to decrypt</param>
        /// <returns>The plaintext</returns>
        public static string Decrypt(string cipherText)
        {
            //byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            //byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            //byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            //PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, "MD5", 1);
            //byte[] keyBytes = password.GetBytes(16);
            //RijndaelManaged symmetricKey = new RijndaelManaged();
            //symmetricKey.Mode = CipherMode.CBC;
            //ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            //MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            //CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            //byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            //int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            //memoryStream.Close();
            //cryptoStream.Close();
            //string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            //return plainText;   
            return "De ";
        }

        
  
    }
}