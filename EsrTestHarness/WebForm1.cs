//// Decompiled with JetBrains decompiler
//// Type: IBM_Scorm_Web.WebForm1
//// Assembly: IBM_Scorm_Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: 12057E26-2372-40A9-ABAF-738EEB30ACDE
//// Assembly location: C:\HEE\R49JSAdapter12_aspnet\bin\IBM_Scorm_Web.dll

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Cache;
//using System.Text;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.HtmlControls;

//namespace IBM_Scorm_Web
//{
//    public class WebForm1 : Page
//    {
//        private bool g_debug_flag = false;
//        private string customDelim = "~~CUSTOM~~";
//        protected HtmlForm form1;

//        private void WriteLog(string message)
//        {
//            try
//            {
//                string path = this.Server.MapPath("CustomLog.txt");
//                DateTime now = DateTime.Now;
//                string str1 = now.ToShortDateString().ToString();
//                now = DateTime.Now;
//                string str2 = now.ToLongTimeString().ToString();
//                string str3 = str1 + " " + str2 + " ==> ";
//                if (!this.g_debug_flag)
//                    return;
//                StreamWriter streamWriter = new StreamWriter(path, true);
//                streamWriter.WriteLine(str3 + message);
//                streamWriter.Flush();
//                streamWriter.Close();
//            }
//            catch (Exception ex)
//            {
//            }
//        }

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            string s = "Start";
//            string str1 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//            HttpRequest request = HttpContext.Current.Request;
//            HttpResponse response1 = HttpContext.Current.Response;
//            try
//            {
//                string query = request.Url.Query;
//                this.WriteLog(query);
//                string uriString1 = query.Substring(8);
//                string str2 = "false";
//                int startIndex1 = uriString1.IndexOf("logDebug");
//                if (startIndex1 > 0)
//                {
//                    int startIndex2 = uriString1.IndexOf("=", startIndex1) + 1;
//                    str2 = uriString1.Substring(startIndex2);
//                    uriString1 = uriString1.Substring(0, uriString1.IndexOf("logDebug") - 1);
//                }
//                this.WriteLog("logDebug = " + str2);
//                if ("true".Equals(str2))
//                {
//                    this.WriteLog("LMS Url set to:" + uriString1);
//                    this.Response.AppendToLog("LMS Url set to:" + uriString1);
//                }
//                else
//                    this.g_debug_flag = false;
//                int contentLength = request.ContentLength;
//                TextReader textReader = (TextReader)new StreamReader((Stream)new BufferedStream(request.InputStream), Encoding.UTF8);
//                string str3 = "";
//                string str4;
//                while ((str4 = textReader.ReadLine()) != null)
//                    str3 = str3 + str4 + " \n ";
//                string str5 = str3.Trim();
//                if ("true".Equals(str2))
//                {
//                    this.WriteLog("message posted by API:" + str5);
//                    this.Response.AppendToLog("message posted by API:" + str5);
//                }
//                string str6 = str5;
//                this.WriteLog("Enc Message " + str6);
//                int num1 = 0;
//                string str7 = "";
//                long num2 = 0;
//                for (int index = str5.Length - 1; index >= 0; --index)
//                {
//                    num2 += (long)str1.IndexOf(str5[index]) * (long)Math.Pow(36.0, (double)num1);
//                    ++num1;
//                    if (Math.IEEERemainder((double)num1, 3.0) == 0.0)
//                    {
//                        str7 = ((char)num2).ToString() + str7;
//                        num1 = 0;
//                        num2 = 0L;
//                    }
//                }
//                string str8 = str7;
//                this.WriteLog("Message " + str8);
//                if (str8.IndexOf("oracle.apps.ota.private.isValidURI") >= 0)
//                {
//                    bool flag = true;
//                    string[] separator = new string[1] { "~olmota~" };
//                    foreach (string str9 in str8.Split(separator, StringSplitOptions.RemoveEmptyEntries))
//                    {
//                        if (str9.StartsWith("oracle.apps.ota.private.isvaliduri"))
//                        {
//                            string uriString2 = str9.Substring(str9.IndexOf(" =") + 1);
//                            if (uriString2 != null)
//                            {
//                                try
//                                {
//                                    Uri uri = new Uri(uriString2);
//                                    flag = true;
//                                }
//                                catch
//                                {
//                                    flag = false;
//                                }
//                            }
//                            string str10 = " oracle.apps.ota.private.isvaliduri=" + (flag ? " true" : "false") + "\nerror_text = \nerror=0";
//                            this.WriteLog("returnMsg " + str10);
//                            StreamWriter streamWriter = new StreamWriter(response1.OutputStream, Encoding.UTF8);
//                            streamWriter.Write(str10);
//                            streamWriter.Flush();
//                            streamWriter.Close();
//                            if ("true".Equals(str2))
//                            {
//                                this.WriteLog("posted message to LMS:" + str10);
//                                this.Response.AppendToLog("posted message to LMS:" + str10);
//                                break;
//                            }
//                            break;
//                        }
//                    }
//                }
//                else
//                {
//                    bool flag = false;
//                    string str9 = (string)null;
//                    string[] separator1 = new string[1] { "~olmota~" };
//                    string[] strArray1 = str8.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
//                    string str10 = "";
//                    for (int index = 0; index < strArray1.Length; ++index)
//                    {
//                        string str11 = strArray1[index];
//                        string str12 = str11.Substring(0, str11.IndexOf("="));
//                        string str13 = str11.Substring(str11.IndexOf("=") + 1);
//                        if (str12.Equals("oracle.apps.ota.private.sessionId"))
//                            str9 = str13;
//                        else if ("action".Equals(str12) && "ExitAU".Equals(str13))
//                            flag = true;
//                        if (str10.Equals(""))
//                            str10 = HttpUtility.UrlEncode(str12) + "=" + HttpUtility.UrlEncode(str13);
//                        else
//                            str10 = str10 + "&" + HttpUtility.UrlEncode(str12) + "=" + HttpUtility.UrlEncode(str13, Encoding.UTF8);
//                    }
//                    HttpContext current = HttpContext.Current;
//                    string str14 = (string)current.Session[str9 + "HDR"];
//                    byte[] bytes = Encoding.UTF8.GetBytes("scormd=" + str6);
//                    Uri uri = new Uri(uriString1);
//                    this.WriteLog("reqUrl " + uriString1);
//                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//                    HttpRequestCachePolicy requestCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
//                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
//                    httpWebRequest.ContentLength = (long)bytes.Length;
//                    httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
//                    httpWebRequest.Method = "POST";
//                    httpWebRequest.UserAgent = "Mozilla/5.0";
//                    httpWebRequest.CachePolicy = (RequestCachePolicy)requestCachePolicy;
//                    if (str14 != null && str14.Length > 0)
//                    {
//                        httpWebRequest.CookieContainer = new CookieContainer();
//                        string[] separator2 = new string[1]
//                        {
//              this.customDelim
//                        };
//                        string[] separator3 = new string[1] { ";" };
//                        string[] separator4 = new string[1] { "=" };
//                        foreach (string str11 in str14.Split(separator2, StringSplitOptions.RemoveEmptyEntries))
//                        {
//                            str14 = str11;
//                            string[] strArray2 = str11.Split(separator3, StringSplitOptions.RemoveEmptyEntries)[0].Split(separator4, StringSplitOptions.RemoveEmptyEntries);
//                            Cookie cookie = new Cookie(strArray2[0], strArray2[1], "/");
//                            httpWebRequest.CookieContainer.Add(uri, cookie);
//                        }
//                        if (this.g_debug_flag)
//                            this.WriteLog(str9 + " cookies in session " + str14);
//                    }
//                    Stream requestStream = httpWebRequest.GetRequestStream();
//                    requestStream.Write(bytes, 0, bytes.Length);
//                    requestStream.Flush();
//                    requestStream.Close();
//                    HttpWebResponse response2 = (HttpWebResponse)httpWebRequest.GetResponse();
//                    string str15 = "";
//                    if (((IEnumerable<string>)response2.Headers.AllKeys).Contains<string>("Set-Cookie"))
//                    {
//                        foreach (string str11 in response2.Headers.GetValues("Set-Cookie"))
//                            str15 = str15 + str11 + this.customDelim;
//                        current.Session[str9 + "HDR"] = (object)str15;
//                        this.WriteLog("cookieStr " + str15);
//                    }
//                    StreamReader streamReader = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);
//                    s = streamReader.ReadToEnd();
//                    this.WriteLog("responseFromServer " + s);
//                    if ("true".Equals(str2))
//                    {
//                        this.WriteLog("response from LMS::" + s);
//                        this.Response.AppendToLog("response from LMS::" + s);
//                    }
//                    response2.Close();
//                    streamReader.Close();
//                    if (flag)
//                        current.Session.Remove(str9 + "HDR");
//                }
//            }
//            catch (Exception ex)
//            {
//                if (this.g_debug_flag)
//                    this.WriteLog(" Exception occurred " + ex.Message);
//            }
//            response1.Write(s);
//            response1.Flush();
//            response1.Close();
//        }
//    }
//}
