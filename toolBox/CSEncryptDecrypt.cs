using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CSEncryptDecrypt
{
    public class Class1
    {
        private string ssecretKey;
        public string sSecretKey
        {
            get
            {
                return ssecretKey;
            }
            set
            {
                ssecretKey = value;
            }
        }

        public Class1()
        {
            sSecretKey = "?d]-+a??|kD";
        }

        public void EncryptFile(string sInputFilename,
           string sOutputFilename,
           string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename,
               FileMode.Open,
               FileAccess.Read);

            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create,
               FileAccess.Write);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt,
               CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }

        public string DecryptFile(string sInputFilename, string sOutputFilename, string sKey, Boolean WithFile = false)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            //A 64 bit key and IV is required for this provider.
            //Set secret key For DES algorithm.
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //Set initialization vector.
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            //Create a file stream to read the encrypted file back.
            FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);

            //Create a DES decryptor from the DES instance.
            ICryptoTransform desdecrypt = DES.CreateDecryptor();

            //Create crypto stream set to read and do a 
            //DES decryption transform on incoming bytes.
            CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            string decryptDesc = new StreamReader(cryptostreamDecr).ReadToEnd();
            if (WithFile)
            {
                StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
                fsDecrypted.Write(decryptDesc);
                fsDecrypted.Flush();
                fsDecrypted.Close();
            }
            return decryptDesc;
        }

        public string DecryptXML(string sInputFilename)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sSecretKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sSecretKey);
            FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            ICryptoTransform desdecrypt = DES.CreateDecryptor();
            CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);
            string decryptDesc = new StreamReader(cryptostreamDecr).ReadToEnd();
            return decryptDesc;
        }
    }
}