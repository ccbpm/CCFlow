using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;

namespace BP.Tools
{
    public static class HttpUtil
    {
        public static byte[] doGet(string url, Dictionary<String, String[]> header, Dictionary<String, String[]> param, string expectContentType)
        {
            try
            {
                StringBuilder content = new StringBuilder();
                content.Append(url);
                using (HttpClient client = new HttpClient())
                {
                    if (param != null)
                    {
                        content.Append("?");
                        int i = 0;
                        foreach (var p in param)
                        {
                            foreach (var v in p.Value)
                            {
                                if (i > 0)
                                {
                                    content.Append("&");
                                }
                                content.AppendFormat("{0}={1}", p.Key, System.Web.HttpUtility.UrlEncode(v));
                                i++;
                            }

                        }
                    }

                    if (header != null)
                    {
                        client.DefaultRequestHeaders.Clear();
                        foreach (var h in header)
                        {
                            foreach (var v in h.Value)
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, v);
                            }

                        }
                    }
                    var res = client.GetAsync(content.ToString()).Result;
                    IEnumerable<string> contentTypeHeader;
                    var suc = res.Content.Headers.TryGetValues("Content-Type", out contentTypeHeader);
                    if (suc && !((string[])contentTypeHeader)[0].Contains(expectContentType))
                    {
                        Console.WriteLine(res.Content.ReadAsStringAsync().Result);
                        return null;
                    }
                    return res.Content.ReadAsByteArrayAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("http request error: " + ex);
                return null;
            }

        }

        public static byte[] doPost(string url, Dictionary<String, String[]> header, Dictionary<String, String[]> param, string expectContentType)
        {
            try
            {
                StringBuilder content = new StringBuilder();
                using (HttpClient client = new HttpClient())
                {
                    if (param != null)
                    {
                        int i = 0;
                        foreach (var p in param)
                        {
                            foreach (var v in p.Value)
                            {
                                if (i > 0)
                                {
                                    content.Append("&");
                                }
                                content.AppendFormat("{0}={1}", p.Key, System.Web.HttpUtility.UrlEncode(v));
                                i++;
                            }

                        }
                    }

                    var para = new StringContent(content.ToString());
                    if (header != null)
                    {
                        para.Headers.Clear();
                        foreach (var h in header)
                        {
                            foreach (var v in h.Value)
                            {
                                para.Headers.Add(h.Key, v);
                            }
                        }
                    }
                    var res = client.PostAsync(url, para).Result;
                    IEnumerable<string> contentTypeHeader;
                    var suc = res.Content.Headers.TryGetValues("Content-Type", out contentTypeHeader);
                    if (suc && !((string[])contentTypeHeader)[0].Contains(expectContentType))
                    {
                        Console.WriteLine(res.Content.ReadAsStringAsync().Result);
                        return null;
                    }
                    return res.Content.ReadAsByteArrayAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("http request error: " + ex);
                return null;
            }

        }
    }
}