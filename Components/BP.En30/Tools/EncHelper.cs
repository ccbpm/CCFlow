using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web;
using BP.Web;
using BP.Sys;
using System.Text;
using System.Security.Cryptography;


namespace BP.Tools
{

    public class EncHelper
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };//自定义密匙

        private static string encryptKey = "ccflow123";

        public static bool EncryptDES(string inFile, string outFile)
        {
            byte[] rgb = Keys;
            try
            {
                byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                FileStream inFs = new FileStream(inFile, FileMode.Open, FileAccess.Read);//读入流
                FileStream outFs = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);// 等待写入流
                outFs.SetLength(0);//帮助读写的变量
                byte[] byteIn = new byte[100];//放临时读入的流
                long readLen = 0;//读入流的长度
                long totalLen = inFs.Length;//读入流的总长度
                int everylen = 0;//每次读入流的长度
                DES des = new DESCryptoServiceProvider();//将inFile加密后放到outFile
                CryptoStream encStream = new CryptoStream(outFs, des.CreateEncryptor(rgb, rgbKeys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    everylen = inFs.Read(byteIn, 0, 100);
                    encStream.Write(byteIn, 0, everylen);
                    readLen = readLen + everylen;
                }
                encStream.Close();
                inFs.Close();
                outFs.Close();
                return true;//加密成功
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message.ToString());
                return false;//加密失败
            }
        }

        public static bool DecryptDES(string inFile, string outFile)
        {
            byte[] rgb = Keys;
            try
            {
                byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                FileStream inFs = new FileStream(inFile, FileMode.Open, FileAccess.Read);//读入流
                FileStream outFs = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);// 等待写入流
                outFs.SetLength(0);//帮助读写的变量
                byte[] byteIn = new byte[100];//放临时读入的流
                long readLen = 0;//读入流的长度
                long totalLen = inFs.Length;//读入流的总长度
                int everylen = 0;//每次读入流的长度
                DES des = new DESCryptoServiceProvider();//将inFile加密后放到outFile
                CryptoStream encStream = new CryptoStream(outFs, des.CreateDecryptor(rgb, rgbKeys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    everylen = inFs.Read(byteIn, 0, 100);
                    encStream.Write(byteIn, 0, everylen);
                    readLen = readLen + everylen;
                }
                encStream.Close();
                inFs.Close();
                outFs.Close();
                encStream.Dispose();
                inFs.Dispose();
                outFs.Dispose();
                return true;//加密成功
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message.ToString());
                return false;//加密失败
            }
        }
    }
}
