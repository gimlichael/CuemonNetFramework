using System;
using System.Net;

namespace Cuemon.Net
{
    /// <summary>
    /// This utility class is designed to make various net related operations easier to work with.
    /// </summary>
    public static class NetUtility
    {
        /// <summary>
        /// Determines whether the provided <see cref="WebRequest"/> successfully can establish a <see cref="WebResponse"/>.
        /// </summary>
        /// <param name="request">The <see cref="WebRequest"/> to check for a <see cref="WebResponse"/>.</param>
        /// <returns>
        /// 	<c>true</c> if the provided <see cref="WebRequest"/> successfully can establish a <see cref="WebResponse"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanAccess(WebRequest request)
        {
            try
            {
                using (GetResponse(request))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the <see cref="WebResponse"/> of the <see cref="WebRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="WebRequest"/> to initiate a <see cref="WebResponse"/> from.</param>
        /// <returns>A <see cref="WebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static WebResponse GetResponse(WebRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            try
            {
                return request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError) { throw; }
                return ex.Response;
            }
        }

        /// <summary>
        /// Gets the <see cref="WebResponse"/> from the asynchronous request returned by the <see cref="WebRequest.BeginGetResponse"/> method.
        /// </summary>
        /// <param name="result">An <see cref="IAsyncResult"/> that references a pending request for a response.</param>
        /// <returns>A <see cref="WebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static WebResponse GetResponse(IAsyncResult result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (!TypeUtility.ContainsType(result.AsyncState, typeof(WebRequest))) { throw new ArgumentException("AsyncState must inherit from the abstract WebRequest type.", "result"); }
            try
            {
                return ((WebRequest)result.AsyncState).EndGetResponse(result);
            }
            catch (WebException ex) // this is done due to vexing exception, eg., bad design from Microsoft. They should have returned StatusCode INSTEAD of throwing an exception! Inspiration from this url: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
            {
                if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError) { throw; }
                return ex.Response;
            }
        }

        /// <summary>
        /// Gets the <see cref="FileWebResponse"/> of the <see cref="FileWebRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="WebRequest"/> to initiate a <see cref="FileWebResponse"/> from.</param>
        /// <returns>A <see cref="FileWebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static FileWebResponse GetFileWebResponse(WebRequest request)
        {
            return (FileWebResponse)GetResponse(request);
        }

        /// <summary>
        /// Gets the <see cref="FileWebResponse"/> from the asynchronous request returned by the <see cref="FileWebRequest.BeginGetResponse"/> method.
        /// </summary>
        /// <param name="result">An <see cref="IAsyncResult"/> that references a pending request for a response.</param>
        /// <returns>A <see cref="FileWebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static FileWebResponse GetFileWebResponse(IAsyncResult result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (result.AsyncState.GetType() != typeof(FileWebRequest)) { throw new ArgumentException("AsyncState must be of FileWebRequest type.", "result"); }
            return (FileWebResponse)GetResponse(result);
        }

        /// <summary>
        /// Gets the <see cref="FtpWebResponse"/> of the <see cref="FtpWebRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="WebRequest"/> to initiate a <see cref="FtpWebResponse"/> from.</param>
        /// <returns>A <see cref="FtpWebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static FtpWebResponse GetFtpWebResponse(WebRequest request)
        {
            return (FtpWebResponse)GetResponse(request);
        }

        /// <summary>
        /// Gets the <see cref="FtpWebResponse"/> from the asynchronous request returned by the <see cref="FtpWebRequest.BeginGetResponse"/> method.
        /// </summary>
        /// <param name="result">An <see cref="IAsyncResult"/> that references a pending request for a response.</param>
        /// <returns>A <see cref="FtpWebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static FtpWebResponse GetFtpWebResponse(IAsyncResult result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (result.AsyncState.GetType() != typeof(FtpWebRequest)) { throw new ArgumentException("AsyncState must be of FtpWebRequest type.", "result"); }
            return (FtpWebResponse)GetResponse(result);
        }

        /// <summary>
        /// Gets the <see cref="HttpWebResponse"/> of the <see cref="HttpWebRequest"/> instance.
        /// </summary>
        /// <param name="request">The <see cref="WebRequest"/> to initiate a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>A <see cref="HttpWebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static HttpWebResponse GetHttpWebResponse(WebRequest request)
        {
            return (HttpWebResponse)GetResponse(request);
        }

        /// <summary>
        /// Gets the <see cref="HttpWebResponse"/> of the asynchronous <see cref="HttpWebRequest"/> instance.
        /// </summary>
        /// <param name="result">An <see cref="IAsyncResult"/> that references a pending HTTP request for a HTTP response.</param>
        /// <returns>A <see cref="WebResponse"/> object, not suffering from the vexing exception design by Microsoft.</returns>
        /// <remarks>
        /// More about vexing design can be found here: http://blogs.msdn.com/ericlippert/archive/2008/09/10/vexing-exceptions.aspx
        /// Thanks for the enlightenment goes to this URL: http://stackoverflow.com/questions/1366848/httpwebrequest-getresponse-throws-webexception-on-http-304
        /// </remarks>
        public static HttpWebResponse GetHttpWebResponse(IAsyncResult result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (result.AsyncState.GetType() != typeof(HttpWebRequest)) { throw new ArgumentException("AsyncState must be of HttpWebRequest type.", "result"); }
            return (HttpWebResponse)GetResponse(result);
        }
    }
}