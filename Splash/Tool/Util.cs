﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace Splash.Tool
{
    class Util
    {
        public static string GetFilterString(string str)
        {
            str = str.Replace('ł', '?');
            str = str.Replace('æ', '?');
            str = str.Replace('і', '?');
            str = str.Replace('Ø', '?');
            str = str.Replace('ø', '?');
            str = str.Replace('‏','?');
            str = str.Replace('ç', '?');
            return str;
        }
        public static int GetCommentInt(string str)
        {
            Match match = Regex.Match(str, @"\.*(\d+)\.*");
            string strValue = match.Groups[1].ToString().Trim();
            int value = Convert.ToInt32(strValue);
            return value;
        }
        public static string GetCommentString(string str)
        {
            Match match = Regex.Match(str, @"(\w*)(//|\.*)\.*");
            string strValue = match.Groups[1].ToString().Trim();
            return strValue;
        }
        public static string GetNumericSymbolString(double num)
        {
            if(num > 0)
            {
                return "+" + num.ToString();
            }
            else if(num < 0)
            {
                return num.ToString();
            }
            return "0";
        }
        public static long GetCurrentTimeStamp()
        {
           return  (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        public static List<string> GetFilesCount(string configDir,string suffix = "*.ini")
        {
            List<string> fileNames = new List<string>();
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(configDir);
            System.IO.FileInfo[] info = dirInfo.GetFiles(suffix);
            int num  =info.Length;//获取某种格式
            for (int i = 0; i < num;i++ )
            {
                fileNames.Add(info[i].Name.Split('.')[0]);
            }
            return fileNames;
        }

        public static List<string> GetFileNames(string configDir, string suffix = "*.ini")
        {
            List<string> fileNames = new List<string>();
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(configDir);
            System.IO.FileInfo[] info = dirInfo.GetFiles(suffix);
            int num = info.Length;//获取某种格式
            for (int i = 0; i < num; i++)
            {
                fileNames.Add(info[i].FullName);
            }
            return fileNames;
        }
    }
}
