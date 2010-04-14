using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Sokoban.Lib.Http
{
    public class HttpReq
    {
        /// <summary>
        /// Sends @postData to the file @file (on server: @file) and returns response of the server
        /// </summary>
        /// <param name="file">file is an URL</param>
        /// <param name="postData">in form: variable=data&amp;variable2=someotherdata;</param>
        /// <returns>Response of the server</returns>
        public static string Request(string file, string postData)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(file);
            httpWebRequest.Method = "POST";

            // Create a new string object to POST data to the Url. 
            // string postData = "name=" + name + "&pass=" + pass; // example
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);

            // Set the content type of the data being posted. 
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            // Set the content length of the string being posted. 
            httpWebRequest.ContentLength = byte1.Length;

            Stream newStream = httpWebRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);
            // Close the Stream object. 
            newStream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));

            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Full URL</param>
        /// <returns>Response of the http server</returns>
        public static string GetRequest(string file)
        {
            string fileContent = "";
            WebClient client = new WebClient();
            fileContent = client.DownloadString(file);

            return fileContent;
        }
    }
}
