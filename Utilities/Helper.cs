using System;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Xml;

namespace conversion_api.Utilities
{
    public static class Helper
    {

        public static DateTime dtShowWord = new DateTime(2001, 1, 1);

        public static string ClearHtmlTag(string strText)
        {
            Regex regexTagStart = new Regex("<([^<^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            strText = regexTagStart.Replace(strText, "");

            return strText;
        }

        public static string GetRandomString(int nLen)
        {
            string strRandomString = "";
            Random ra = new Random();
            for (int i = 0; i < nLen; i++)
            {
                strRandomString += ra.Next(0, 10).ToString();
            }
            return strRandomString;
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
        /// </summary>
        /// <param name="toEncrypt">string to be encrypted</param>
        /// <param name="useHashing">use hashing? send to for extra secirity</param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file
            string key = "abcdefghijklmnop";
            //System.Windows.Forms.MessageBox.Show(key);
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }
        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();

            string key = "abcdefghijklmnop";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        //public static void DownloadFile(string fname, bool forceDownload)
        //{
        //    string path = "";// HttpContext.Current.Server.MapPath(fname);
        //    string name = Path.GetFileName(path);
        //    string ext = Path.GetExtension(path);
        //    string type = "";
        //    // set known types based on file extension  
        //    if (ext != null)
        //    {
        //        switch (ext.ToLower())
        //        {
        //            case ".htm":
        //            case ".html":
        //                type = "text/HTML";
        //                break;

        //            case ".txt":
        //                type = "text/plain";
        //                break;

        //            case ".docx":
        //            case ".doc":
        //            case ".rtf":
        //                type = "Application/msword";
        //                break;
        //            case ".xsl":
        //            case ".xslx":
        //                type = "Application/x-msexcel";
        //                break;
        //            case ".mp3":
        //                type = "audio/mpeg";
        //                break;
        //            case ".mp4":
        //                type = "video/mp4";
        //                break;
        //            case ".jpg":
        //                type = "image/jpeg";
        //                break;
        //            case ".pdf":
        //                type = "application/pdf";
        //                break;
        //            case ".zip":
        //                type = "application/zip";
        //                break;
        //            case ".wma":
        //                type = "audio/x-ms-wma";
        //                break;
        //            case ".rm":
        //                type = "audio/vnd.rn-realaudio";
        //                break;
        //            case ".wav":
        //                type = "audio/x-wav";
        //                break;
        //            case ".wmv":
        //                type = "video/x-ms-wmv";
        //                break;
        //            case ".ppt":
        //            case ".pptx":
        //                type = "application/vnd.ms-powerpoint";
        //                break;
        //            case ".xml":
        //                type = "text/xml";
        //                break;
        //            case ".gif":
        //                type = "image/gif";
        //                break;
        //            case ".png":
        //                type = "image/png";
        //                break;
        //        }
        //    }
        //    if (forceDownload)
        //    {
        //        HttpContext.Current.Response.AppendHeader("content-disposition",
        //            "attachment; filename=" + name);
        //    }
        //    if (type != "")
        //        HttpContext.Current.Response.ContentType = type;
        //    HttpContext.Current.Response.WriteFile(path);
        //    HttpContext.Current.Response.End();
        //}

        public static string HtmlTrimmer(string input, int len)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            if (input.Length <= len)
                return input;

            // this is necissary because regex "^"  applies to the start of the string, not where you tell it to start from
            string inputCopy;
            string tag;

            string result = "";
            int strLen = 0;
            int strMarker = 0;
            int inputLength = input.Length;

            Stack stack = new Stack(10);
            Regex text = new Regex("^[^<&]+");
            Regex singleUseTag = new Regex("^<[^>]*?/>");
            Regex specChar = new Regex("^&[^;]*?;");
            Regex htmlTag = new Regex("^<.*?>");

            while (strLen < len)
            {
                inputCopy = input.Substring(strMarker);
                //If the marker is at the end of the string OR 
                //the sum of the remaining characters and those analyzed is less then the maxlength
                if (strMarker >= inputLength || (inputLength - strMarker) + strLen < len)
                    break;

                //Match regular text
                result += text.Match(inputCopy, 0, len - strLen);
                strLen += result.Length - strMarker;
                strMarker = result.Length;

                inputCopy = input.Substring(strMarker);
                if (singleUseTag.IsMatch(inputCopy))
                    result += singleUseTag.Match(inputCopy);
                else if (specChar.IsMatch(inputCopy))
                {
                    //think of &nbsp; as 1 character instead of 5
                    result += specChar.Match(inputCopy);
                    ++strLen;
                }
                else if (htmlTag.IsMatch(inputCopy))
                {
                    tag = htmlTag.Match(inputCopy).ToString();
                    //This only works if this is valid Markup...
                    if (tag[1] == '/')                 //Closing tag
                        stack.Pop();
                    else                                    //not a closing tag
                        stack.Push(tag);
                    result += tag;
                }
                else    //Bad syntax
                    result += input[strMarker];

                strMarker = result.Length;
            }

            while (stack.Count > 0)
            {
                tag = stack.Pop().ToString();
                result += tag.Insert(1, "/");
            }
            if (strLen == len)
                result += "...";
            return result;
        }

        //public static string GetClientIP(HttpRequest Request)
        //{
        //    string clientIP;
        //    string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (!string.IsNullOrEmpty(ip))
        //    {
        //        string[] ipRange = ip.Trim().Split(',');
        //        int le = ipRange.Length - 1;
        //        clientIP = ipRange[le];
        //    }
        //    else
        //    {
        //        clientIP = Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    return clientIP;
        //}

        public static string GenerateShortUrl(string longUrl)
        {
            String shortURL = "";
            try
            {
                String queryURL = "http://mbeo.co/API.asmx/CreateShortUrl?real_url=" + longUrl;

                WebClient wc = new WebClient();
                string response = wc.DownloadString(queryURL);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                dynamic ShorturlResponse = JsonConvert.DeserializeObject(jsonText);
                if (ShorturlResponse != null)
                {
                    shortURL = ShorturlResponse.Container.ShortUrl;
                }
            }
            catch
            {
            }

            return shortURL;
        }

        public static string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch
            {
                return data;
            }
        }

        public static string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch
            {
                return data;
            }
        }

        public static bool IsNumeric(string number)
        {
            Regex reNum = new Regex(@"^\d+$");
            bool result = reNum.Match(number).Success;
            return result;
        }

        public static byte[] readUrl(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            byte[] imageAsByteArray;
            try
            {
                using (var webClient = new WebClient())
                {
                    imageAsByteArray = webClient.DownloadData(url);
                }
                return imageAsByteArray;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                try
                {
                    using (var webClient = new WebClient())
                    {
                        imageAsByteArray = webClient.DownloadData(url);
                    }
                    return imageAsByteArray;
                }
                catch (Exception ex1)
                {
                    error = ex1.Message;
                    try
                    {
                        using (var webClient = new WebClient())
                        {
                            imageAsByteArray = webClient.DownloadData(url);
                        }
                        return imageAsByteArray;
                    }
                    catch (Exception ex2)
                    {
                        error = ex2.Message;
                        try
                        {
                            using (var webClient = new WebClient())
                            {
                                imageAsByteArray = webClient.DownloadData(url);
                            }
                            return imageAsByteArray;
                        }
                        catch (Exception ex3)
                        {
                            error = ex3.Message;
                            try
                            {
                                using (var webClient = new WebClient())
                                {
                                    imageAsByteArray = webClient.DownloadData(url);
                                }
                                return imageAsByteArray;
                            }
                            catch (Exception ex4)
                            {
                                error = ex4.Message;
                                // returning empty bytes
                                return new byte[0];
                            }
                        }
                    }
                }
            }
        }

    }
}
