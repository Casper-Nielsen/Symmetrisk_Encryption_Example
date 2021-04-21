using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Symmetrisk_Encryption_Example
{
    class Encryption
    {
        private SymmetricAlgorithm symmetric;
        private byte[] key;
        private byte[] iv;

        public byte[] Key
        {
            get { return key; }
            set { key = value; }
        }
        public byte[] IV
        {
            get { return iv; }
            set { iv = value; }
        }
        public Encryption()
        {
            SelectAlgorithm(EncryptionTypeEnum.AES);
            GenerateSet(128);
        }
        public void SelectAlgorithm(EncryptionTypeEnum algorithm)
        {
            symmetric = algorithm switch
            {
                EncryptionTypeEnum.AES => Aes.Create(),
                EncryptionTypeEnum.DES => DES.Create(),
                EncryptionTypeEnum.TripleDES => TripleDES.Create(),
                EncryptionTypeEnum.Rijndael => Rijndael.Create(),
                _ => new AesManaged(),
            };
            symmetric.Padding = PaddingMode.PKCS7;
            symmetric.Mode = CipherMode.CBC;
        }
        public void GenerateSet(int keySize)
        {
            key = new byte[keySize / 8];
            symmetric.GenerateIV();
            iv = symmetric.IV;
            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }
        }

        /// <summary>
        /// encrypts the message with the selected encryption type
        /// </summary>
        /// <param name="message">the message that will be encrypted</param>
        /// <returns>the encrypted message</returns>
        public byte[] Encrypt(byte[] message)
        {
            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = symmetric.CreateEncryptor(Key, IV);
            
            // Create the streams used for EncryptionCore.
            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(message);
            }
            return msEncrypt.ToArray();
            // Return encrypted data    
        }

        /// <summary>
        /// Decrypts a message with the selected encryption type
        /// </summary>
        /// <param name="encryptedMessage">the encrypted message that will be decrypted</param>
        /// <returns>the decrypted message</returns>
        public byte[] Decrypt(byte[] encryptedMessage)
        {
            byte[] rawData = new byte[encryptedMessage.Length];
            try
            {
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = symmetric.CreateDecryptor(Key, IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptedMessage))
                {
                    using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using BinaryReader br = new BinaryReader(csDecrypt);
                    int lenght = br.Read(rawData);
                    Array.Resize(ref rawData, lenght);
                }
            }
            catch { }
            return rawData;
        }
    }
}
