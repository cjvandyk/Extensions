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
    /// <summary>
    /// Extension methods for the System.Net.WebException class.
    /// </summary>
    public static class WebExceptionExtensions
    {
        #region Retry()
        /// <summary>
        /// This extension of the System.Net.WebException class allows
        /// developers an easy way to handle 429 Throttling errors.  When
        /// the WebException is caught, you simply call Retry() on the 
        /// exception, passing the request to it.  The handler will inspect
        /// the HTTP Header for the "Retry-After" property and will sleep
        /// the current thread for that period of time plus 1 minute.  Once
        /// the thread reanimates, it will retry the request again.  If 
        /// further throttling is encountered, the code will recursively
        /// sleep until the request is successfully completed.
        /// When no "Retry-After" header property exist, the exception is
        /// simply rethrown.
        /// </summary>
        /// <param name="ex">The WebException thrown by throttling.</param>
        /// <param name="request">The HttpWebRequest that caused the exception
        /// to be thrown in the first place.</param>
        /// <returns>The HttpWebResponse from the successful completion of
        /// the HttpWebRequest.</returns>
        public static System.Net.HttpWebResponse Retry(
            this System.Net.WebException ex,
            System.Net.HttpWebRequest request)
        {
            Universal.ValidateNoNulls(System.Reflection.MethodInfo.GetCurrentMethod().GetParameters());
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
        #endregion Retry()
    }
}
