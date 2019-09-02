
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Custom.Tool.AutoBuild
{
    public static class PasswordManager
    {
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string GetPassword(string name)
        {
#if UNITY_2019            //check if password is already created
            if(PlayerSettings.Android.useCustomKeystore == true && PasswordExists(name))
#else
            if ((!PlayerSettings.Android.keystoreName.Contains("debug") || PlayerSettings.Android.keystoreName != "") && PasswordExists(name))
#endif
            { 
                //Get Password
                string pass = FileReaderWriter.ReadLineFromFile(Directory.GetCurrentDirectory() + "/tmp/" + name);
                return Decrypt(pass);
            }
            else
            {
                //if not then do this shit
                PasswordPopUpWindow.OpenWindow();
                //return the password
                return "";
            }
        }

        public static void SavePassword(string pass, string name)
        {
            string encryptKey = Encrypt(pass);
            string path = Directory.GetCurrentDirectory() + "/tmp";

            FileReaderWriter.CreateFile(path, name);
            FileReaderWriter.WriteToFile(path + "/" + name, encryptKey);

            encryptKey = "";
        }

        private static bool PasswordExists(string name)
        {
            if (FileReaderWriter.CheckIfFileExists(Directory.GetCurrentDirectory() + "/tmp/" + name))
                return true;
            return false;
        }

        private static string Encrypt(string plainText)
        {
            if (plainText.Length <= 0)
            {
                Debug.LogError("<b><color=red> Text to be encrypted is empty </color></b>");
                return "";
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            byte[] cipherTextBytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        private static string Decrypt(string encryptedText)
        {
            if (encryptedText.Length <= 0)
            {
                Debug.LogError("<b><color=red> Text to be decrypted is empty </color></b>");
                return "";
            }
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}
