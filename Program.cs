using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace XorFileExport
{
    class Program
    {
        static byte[] rawData;
        static byte[] encryptedData;
        static byte[] decryptedData;
        static byte[] key;

        static void XorKeygen(int length)
        {
            Random Rnd = new Random();
            for (int i = 0; i < length; i++)
                key[i] = (byte)Rnd.Next(0, 255);
        }

        static bool XorData(string filename)
        {
            try
            {
                rawData = File.ReadAllBytes(filename);
                encryptedData = rawData;
                for (int i = 0; i < rawData.Length; i++)
                {
                    encryptedData[i] ^= key[i % key.Length];
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("[-]\tError: {0}", e.Message);
                return false;
            }
        }

        static bool ExportFiles()
        {
            try
            {
                using (StreamWriter streamData = new StreamWriter("encryptedData.c"))
                {
                    StringBuilder data = new StringBuilder();
                    data.AppendFormat("unsigned char encryptedData[{0}] = {{\n", encryptedData.Length);
                    
                    for (int i=0;i < encryptedData.Length; i++)
                    {
                        data.AppendFormat("0x{0:x}, ", encryptedData[i]);
                        if ((i % 16) == 0 && i != 0) data.Append("\n");
                    }
                    data.Remove(data.Length - 2, 1);
                    data.Append("};");
                    streamData.Write(data);
                    streamData.Close();
                }
                using (StreamWriter streamKey = new StreamWriter("key.c"))
                {
                    StringBuilder keyB = new StringBuilder();
                    keyB.AppendFormat("unsigned char key[{0}] =  {{\n", key.Length);

                    for (int i = 0; i < key.Length; i++)
                    {
                        keyB.AppendFormat("0x{0:x}, ", key[i]);
                        if ((i % 16) == 0 && i != 0) keyB.Append("\n");
                    }
                    keyB.Remove(keyB.Length - 2, 1);
                    keyB.Append("};");
                    streamKey.Write(keyB);

                    streamKey.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[-]\tError: {0}", e.Message);
                return false;
            }
        }

        static void Main(string[] args)
        {                     
#if DEBUG
            String file = "pupyx86.dll";
            int keySize = 10;
#else
            if (args.Count() < 2)
            {
                Console.WriteLine("[-]\tError: specify inputfile and key size");
                return;
            }
            String file = args[0];
            int keySize = Convert.ToInt32(args[1]);
#endif
            
            key = new byte[keySize];
            try
            {               
                XorKeygen(key.Length);
                if (XorData(file))
                {
                    Console.WriteLine("[+]\tEncryption done");
                    if (ExportFiles())
                        Console.WriteLine("[+]\tFiles written");
                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("[-]\tError: {0}", e.Message);
                return;
            }
        }
    }
}
