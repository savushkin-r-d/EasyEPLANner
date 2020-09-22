﻿using System.IO;
using System.Text;

namespace EasyEPlanner
{
    /// <summary>
    /// Класс-менеджер кодировки проекта
    /// </summary>
    public static class EncodingDetector
    {
        public static Encoding DetectFileEncoding(string pathToFile)
        {
            if(File.Exists(pathToFile))
            {
                var baseFileInfo = new FileInfo(pathToFile);
                long fileSize = baseFileInfo.Length;

                var utf8FileReader = new StreamReader(pathToFile,
                    UTF8);
                string utf8FileData = utf8FileReader.ReadToEnd();
                utf8FileReader.Close();
                const string utf8FileName = "testUTF8.lua";
                File.WriteAllText(utf8FileName, utf8FileData, UTF8);
                var utf8FileInfo = new FileInfo(utf8FileName);
                long utf8FileSize = utf8FileInfo.Length;
                File.Delete(utf8FileName);

                if(fileSize == utf8FileSize)
                {
                    return UTF8;
                }
                else
                {
                    return Windows1251;
                }
            }
            else
            {
                return UTF8;
            }
        }

        public static Encoding Windows1251 
        { 
            get
            {
                return Encoding.GetEncoding(1251);
            } 
        }

        public static Encoding UTF8
        {
            get
            {
                return new UTF8Encoding(false);
            }
        }

        public static Encoding UTF8Bom
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
