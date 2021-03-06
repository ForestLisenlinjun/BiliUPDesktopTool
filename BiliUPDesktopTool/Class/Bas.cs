﻿using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BiliUPDesktopTool
{
    /// <summary>
    /// 公共模块
    /// </summary>
    internal class Bas
    {
        #region Public Fields

        /// <summary>
        /// 主程序Build
        /// </summary>
        public const int _Build = 13;

        /// <summary>
        /// 主程序版本号
        /// </summary>
        public const string _Version = "2.0.0.13 Preview 5";

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// 主程序Build
        /// </summary>
        public static int Build
        {
            get
            {
                return _Build;
            }
        }

        /// <summary>
        /// 开源许可
        /// </summary>
        public static string Thanks
        {
            get
            {
                return Properties.Resources.Thanks;
            }
        }

        /// <summary>
        /// 主程序版本号
        /// </summary>
        public static string Version
        {
            get
            {
                return _Version;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 获取文件md5
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>md5</returns>
        public static string GetFileHash(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取指定url的内容
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="Cookies">cookies</param>
        /// <returns>返回内容</returns>
        public static string GetHTTPBody(string url, string Cookies = "", string Referer = "")
        {
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            StreamReader reader = null;
            string body = "";
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);

                if (!string.IsNullOrEmpty(Cookies))
                {
                    CookieCollection CookiesC = SetCookies(Cookies, url);
                    req.CookieContainer = new CookieContainer(CookiesC.Count)
                    {
                        PerDomainCapacity = CookiesC.Count
                    };
                    req.CookieContainer.Add(CookiesC);
                }

                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                req.Referer = Referer;

                rep = (HttpWebResponse)req.GetResponse();
                reader = new StreamReader(rep.GetResponseStream());
                body = reader.ReadToEnd();
            }
            catch
            {
            }
            finally
            {
                if (reader != null) reader.Close();
                if (rep != null) rep.Close();
                if (req != null) req.Abort();
            }
            return body;
        }

        /// <summary>
        /// 会抛出错误的获取指定url的内容
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="Cookies">cookies</param>
        /// <returns>返回内容</returns>
        public static string GetHTTPBodyThrow(string url, string Cookies = "", string Referer = "")
        {
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            StreamReader reader = null;
            string body = "";
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);

                if (!string.IsNullOrEmpty(Cookies))
                {
                    CookieCollection CookiesC = SetCookies(Cookies, url);
                    req.CookieContainer = new CookieContainer(CookiesC.Count)
                    {
                        PerDomainCapacity = CookiesC.Count
                    };
                    req.CookieContainer.Add(CookiesC);
                }

                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                req.Referer = Referer;

                rep = (HttpWebResponse)req.GetResponse();
                reader = new StreamReader(rep.GetResponseStream());
                body = reader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (rep != null) rep.Close();
                if (req != null) req.Abort();
            }
            return body;
        }

        /// <summary>
        /// POST方法
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">要post的数据</param>
        /// <param name="Cookies">cookies</param>
        /// <returns>返回内容</returns>
        public static string PostHTTPBody(string url, string data = "", string Cookies = "", string Referer = "", string ContentType = "application/x-www-form-urlencoded; charset=UTF-8")
        {
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            StreamReader reader = null;
            string body = "";
            byte[] bdata = Encoding.UTF8.GetBytes(data);
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";

                if (!string.IsNullOrEmpty(Cookies))
                {
                    CookieCollection CookiesC = SetCookies(Cookies, url);
                    req.CookieContainer = new CookieContainer(CookiesC.Count)
                    {
                        PerDomainCapacity = CookiesC.Count
                    };
                    req.CookieContainer.Add(CookiesC);
                }

                req.Accept = "*/*";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                req.Referer = Referer;
                req.ContentType = ContentType;

                Stream writer = req.GetRequestStream();
                writer.Write(bdata, 0, bdata.Length);
                writer.Close();

                rep = (HttpWebResponse)req.GetResponse();
                reader = new StreamReader(rep.GetResponseStream());
                body = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
            }
            finally
            {
                if (reader != null) reader.Close();
                if (rep != null) rep.Close();
                if (req != null) req.Abort();
            }
            return body;
        }

        /// <summary>
        /// 设置Cookies
        /// </summary>
        /// <param name="cookiestr">Cookies的字符串</param>
        public static CookieCollection SetCookies(string cookiestr, string url)
        {
            try
            {
                CookieCollection public_cookie;
                Uri target = new Uri(url);
                public_cookie = new CookieCollection();
                cookiestr = cookiestr.Replace(",", "%2C");//转义“，”
                string[] cookiestrs = Regex.Split(cookiestr, "; ");
                foreach (string i in cookiestrs)
                {
                    string[] cookie = Regex.Split(i, "=");
                    public_cookie.Add(new Cookie(cookie[0], cookie[1]) { Domain = target.Host });
                }
                return public_cookie;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 解压功能
        /// </summary>
        /// <param name="fileToUnZip">待解压的文件</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <param name="password">密码</param>
        /// <returns>解压结果</returns>
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip.Trim()));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        using (fs = File.Create(fileName))
                        {
                            int size = 2048;
                            byte[] data = new byte[size];
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                if (size > 0)
                                    fs.Write(data, 0, size);
                                else
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>
        /// 用户统计
        /// </summary>
        public static void User_Statistics()
        {
            string cpu = MachineInfoHelper.GetCPUInfo();
            string drive = MachineInfoHelper.GetMainDriveId();
            string info = EncryptHelper.GetMD5_16(cpu + drive);
            string json = "{\"pid\":117,\"version\":" + Build + ",\"token\":\"" + info + "\"}";
            PostHTTPBody("https://cloud.api.zhangbudademao.com/public/User_Statistics", json, "", "application/json; charset=UTF-8");
        }

        #endregion Public Methods
    }
}