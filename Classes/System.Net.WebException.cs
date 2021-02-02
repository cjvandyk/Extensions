#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060, IDE0079 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args, Remove unnecessary suppression)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

namespace Extensions
{
    public static class WebException
    {
        public static System.Net.HttpWebResponse Retry(
            this System.Net.WebException ex,
            System.Net.HttpWebRequest request)
        {
            //Check if the response header contains a Retry-After value.
            if (!string.IsNullOrEmpty(ex.Response.Headers["Retry-After"]))
            {
                bool executeOK = false;
                string accept = request.Accept;
                string method = request.Method;
                while (!executeOK)
                {
                    System.Threading.Thread.Sleep((
                        Convert.ToInt32(
                            ex.Response.Headers["Retry-After"]) + 60) * 1000);
                    //We add 60 seconds to the wait time for good measure.
                    request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(
                        ex.Response.ResponseUri.ToString()
                                               .TrimStart('{')
                                               .TrimEnd('}'));
                    request.Method = method;
                    request.Accept = accept;
                    System.Net.WebHeaderCollection headers;
                    System.Net.WebResponse response;
                    try
                    {
                        response = request.GetResponse();
                        executeOK = true;
                        return response as System.Net.HttpWebResponse;
                    }
                    catch (System.Net.WebException ex2)
                    {
                        return ex2.Retry(request);
                    }
                }
            }
            throw ex;  //There is no Retry-After header so just rethrow.
        }
    }
}
