using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WebRanking.Model;

namespace WebRanking.BAL
{
    public class WebRankManager
    {
        public WebRankManager(WebRank webRank)
        {            
             WebRank = webRank;
        }

        WebRank WebRank { get; set; }

        public void GetWebRank()
        {
            var result = new List<int>();
            if (WebRank != null && !string.IsNullOrEmpty(WebRank.SearchKeyWord) && !string.IsNullOrEmpty(WebRank.SearchURL))
            {
                var urlCount = 0;
                var regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);

                var responseStreamAsString = GetHttpResponse();
                try
                {
                    var matches = regex.Matches(responseStreamAsString);
                    if (matches != null)
                    {
                        foreach (Match match in matches)
                        {
                            var hrefValue = match.Groups[1].Value;
                            if (!hrefValue.ToString().ToUpper().Contains("GOOGLE")
                                && hrefValue.ToString().Contains("/url?q=")
                                && (hrefValue.ToString().ToUpper().Contains("HTTP://") || hrefValue.ToString().ToUpper().Contains("HTTPS://")))
                            {
                                int index = hrefValue.IndexOf("&");
                                if (index > 0)
                                {
                                    hrefValue = hrefValue.Substring(0, index);
                                    if (hrefValue.ToUpper().Contains(WebRank.SearchURL.ToUpper()) && urlCount <= 100)
                                    {
                                        result.Add(urlCount);
                                    }
                                    urlCount++;
                                }
                            }
                        }                      
                    }


                }
                catch (Exception)
                {
                    throw new Exception("Error finding match.");
                }
                finally
                {

                }
            }

            if (result != null && result.Any())
            {
                WebRank.SearchResult = string.Join(" , ", result);
            }
        }

        protected virtual string GetHttpResponse()
        {
            var responseStreamAsString = "";
            if (WebRank != null && !string.IsNullOrEmpty(WebRank.SearchKeyWord))
            {
                try
                {
                    string searchResults = "http://google.com/search?num=100&q=" + WebRank.SearchKeyWord; //The google address is hardcoded here, Can be taken as param
                    var request = (HttpWebRequest)WebRequest.Create(searchResults);
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                responseStreamAsString = readStream.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error getting http response.");
                }
                finally
                {

                }
            }
            return responseStreamAsString;
        }

    }

}
