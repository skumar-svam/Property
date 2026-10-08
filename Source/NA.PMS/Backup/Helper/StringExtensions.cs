using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NA.PMS.NICServices
{
    /// <summary>
    /// Extensions for <see cref="System.String"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A nicer way of calling <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// A nicer way of calling the inverse of <see cref="System.String.IsNullOrEmpty(string)"/>
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !value.IsNullOrEmpty();
        }

        /// <summary>
        /// A nicer way of calling <see cref="System.String.Format(string, object[])"/>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Allows for using strings in null coalescing operations
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>Null if <paramref name="value"/> is empty or the original value of <paramref name="value"/>.</returns>
        public static string NullIfEmpty(this string value)
        {
            if (value == string.Empty)
                return null;

            return value;
        }


        

        private static bool IsWordChar(char c)
        {
            return Char.IsLetterOrDigit(c) || c == '_';
        }
        public static string ReplaceFullWords(this string s, string oldWord, string newWord)
        {
            if (s == null)
            {
                return null;
            }
            int startIndex = 0; // Where we start to search in s.
            int copyPos = 0; // Where we start to copy from s to sb.
            var sb = new StringBuilder();
            while (true)
            {
                int position = s.IndexOf(oldWord, startIndex);
                if (position == -1)
                {
                    if (copyPos == 0)
                    {
                        return s;
                    }
                    if (s.Length > copyPos)
                    { // Copy last chunk.
                        sb.Append(s.Substring(copyPos, s.Length - copyPos));
                    }
                    return sb.ToString();
                }
                int indexAfter = position + oldWord.Length;
                if ((position == 0 || !IsWordChar(s[position - 1])) && (indexAfter == s.Length || !IsWordChar(s[indexAfter])))
                {
                    sb.Append(s.Substring(copyPos, position - copyPos)).Append(newWord);
                    copyPos = position + oldWord.Length;
                }
                startIndex = position + oldWord.Length;
            }
        }
        public static Boolean IsStringGuid(String value)
        {

            // this is before the overhead of setting up the try/catch block.
            if (value == null || value.Length != 36)
            {
                return false;
            }

            Guid guid;
            return Guid.TryParse(value, out guid);
        }

        public static String ToMD5HashForPassword(this String plainText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(plainText));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        //#region Encription
        public static string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string RemoveWhitespace(this string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        
    }


}
