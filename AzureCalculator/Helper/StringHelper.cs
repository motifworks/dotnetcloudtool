using AzureCalculator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AzureCalculator.Helper
{
    public class StringHelper
    {
        private static Char[] LETTER_OR_DIGIT = new Char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static String GenerateRandomString(int size, int start, int max)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(max * random.NextDouble() + start)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static String RemoveSpecialCharacters(String input)
        {
            char[] buffer = new char[input.Length];
            int index = 0;
            foreach (char c in input)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
                {
                    buffer[index] = c;
                    index++;
                }
            }
            return new String(buffer, 0, index);
        }

        public static String ReplaceParameter(String input, User user)
        {
            input = input.Replace("{UserName}", user.UserName);
            input = input.Replace("{SiteName}", user.SiteName);
            input = input.Replace("{LoginId}", user.LoginId);
            input = input.Replace("{LoginPassword}", user.LoginPassword);
            input = input.Replace("{Password}", user.LoginPassword);
            return input;
        }

        public static String CreateQualifiedFileName(String path, String fileName)
        {
            return CreateQualifiedFileName(path, "", fileName);
        }

        private static string AppendPathSeparatorIfNotPresentAtEnd(String path)
        {
            if (!path.EndsWith("" + Path.DirectorySeparatorChar))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        public static String CreateQualifiedFileName(String basePath, String path, String fileName)
        {
            basePath = ReplacePathSeparator(basePath);
            path = ReplacePathSeparator(path);

            if (String.IsNullOrEmpty(basePath))
            {
                basePath = ".";
            }

            if (!String.IsNullOrEmpty(path))
            {
                basePath = AppendPathSeparatorIfNotPresentAtEnd(basePath);
                basePath += path;
            }

            if (!String.IsNullOrEmpty(fileName))
            {
                basePath = AppendPathSeparatorIfNotPresentAtEnd(basePath);
                basePath += fileName;
            }

            return basePath;
        }

        public static string ReplacePathSeparator(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return path;
            }

            if ('\\' != Path.DirectorySeparatorChar && path.Contains("\\"))
            {
                path = path.Replace("\\", "" + Path.DirectorySeparatorChar);
            }

            if ('/' != Path.DirectorySeparatorChar && path.Contains("/"))
            {
                path = path.Replace("/", "" + Path.DirectorySeparatorChar);
            }
            return path;
        }
    }
}