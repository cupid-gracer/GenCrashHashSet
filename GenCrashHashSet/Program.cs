using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GenCrashHashSet
{
    class Program
    {
        static void Main(string[] args)
        {
            string next_hash = "9cb5e176e918ae819bbf64008f8bc4213d2882e5a9e6a9a13b62b60291ec5a46";
           
            int unit_num = 500000, now_number = 4587145;
            bool isAll = false;
            string result_folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/result/";
            string prev_hash = "";
            for (int step = now_number / unit_num;  step >= 0; step--)
            {
                string filePath = result_folder+ "hash_set__" +step.ToString()+".csv";
                var csv = new StringBuilder();

                for (int i = 0; i < unit_num; i++)
                {
                    //Console.WriteLine(now_number);
                    if(now_number < 0) { isAll = true; break; }
                    prev_hash = sha256(next_hash);
                    string payout = getCrash(next_hash);
                    var newLine = string.Format("{0},{1},{2}", now_number, next_hash, payout);
                    csv.AppendLine(newLine);
                    next_hash = prev_hash;
                    now_number = now_number - 1;
                }
                File.WriteAllText(filePath, csv.ToString());
                if (isAll) break;
                break;
            }
            //after your loop
        }

        static string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }

        static String HashHMACHex(String text, String key = "0000000000000000000e3a66df611d6935b30632f352e4934c21059696117f28")
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();
            
            Byte[] textBytes = StringToByteArray(text);



            Byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            string result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return result;
        }

        static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }

        static String getCrash(string seed, int nBits = 52)
        {
            seed = HashHMACHex(seed);
            seed = seed.Substring(0, nBits / 4);
            Int64 r = Convert.ToInt64(seed, 16);

            // 3. X = r / 2^52
            double X = r / Math.Pow(2, nBits); // uniformly distributed in [0; 1)
            //X = X.toPrecision(9));
            
            // 4. X = 99 / (1-X)
            X = 99 / (1 - X);

            // 5. return max(trunc(X), 100)
            double result = Math.Floor(X);
            return Math.Max(1, result / 100).ToString();

        }

    }
}
