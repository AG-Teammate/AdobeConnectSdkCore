namespace AdobeConnectSDK.Common
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using AdobeConnectSDK.Interfaces;
    using AdobeConnectSDK.Model;
    using Octopus.System.Xml;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Linq;

    public class HttpCommunicationProvider : ICommunicationProvider
    {
        private string m_SessionInfo = string.Empty;
        private string m_SessionDomain = string.Empty;

        public ISdkSettings Settings { get; set; }

        public async Task<ApiStatus> ProcessRequest(string pAction, string qParams)
        {
            if (qParams == null)
                qParams = string.Empty;
            var url = string.Format(@"?action={0}&{1}", pAction, qParams);

            if (!Settings.ServiceURL.EndsWith("/api/xml"))
            {
                url = "/api/xml" + url;
            }

            ApiStatus operationApiStatus = new ApiStatus();
            operationApiStatus.Code = StatusCodes.NotSet;

            var receiveStream = await ProcessRequestInternal(url);

            using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
            {
                string buf = await readStream.ReadToEndAsync();
                operationApiStatus = Helpers.ResolveOperationStatusFlags(new XmlTextReader(new StringReader(buf), true)); //AG: not sure
            }

            if (this.Settings.UseSessionParam)
            {
                operationApiStatus.SessionInfo = this.m_SessionInfo;
            }

            return operationApiStatus;
        }

        private async Task<Stream> ProcessRequestInternal(string url)
        {
            if (this.Settings == null)
            {
                throw new InvalidOperationException("This provider is not configured.");
            }

            var handler = new HttpClientHandler();
            var client = new HttpClient(handler);

            client.Timeout = new TimeSpan(0, 0, 20);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            handler.CookieContainer = new CookieContainer();

            if (!this.Settings.UseSessionParam)
            {
                if (!string.IsNullOrEmpty(m_SessionInfo) && !string.IsNullOrEmpty(m_SessionDomain))
                    handler.CookieContainer.Add(new Uri(this.Settings.ServiceURL), new Cookie("BREEZESESSION", this.m_SessionInfo, "/", this.m_SessionDomain));
            }

            var response = await client.GetAsync(this.Settings.ServiceURL + url);

            try
            {
                if (this.Settings.UseSessionParam)
                {
                    var cookies = handler.CookieContainer.GetCookies(new Uri(this.Settings.ServiceURL)).Cast<Cookie>();
                    var sessionCookie = cookies.FirstOrDefault(c => c.Name == "BREEZESESSION");
                    if (sessionCookie != null)
                    {
                        this.m_SessionInfo = sessionCookie.Value;
                        this.m_SessionDomain = sessionCookie.Domain;
                    }
                }

                Stream receiveStream = await response.Content.ReadAsStreamAsync();
                if (receiveStream == null)
                    return null;

                return receiveStream;
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                throw ex.InnerException;
            }
            //finally
            //{
            //    if (HttpWResp != null)
            //        HttpWResp.Close();
            //}
        }

        public async Task<byte[]> DownloadContent(string url, string sessionInfo = null)
        {
            if (!string.IsNullOrEmpty(sessionInfo) && Settings.UseSessionParam)
            {
                url += (url.Contains("?") ? "&" : "?") + "session=" + sessionInfo;
            }

            using (var stream = await ProcessRequestInternal(url))
            using (var reader = new BinaryReader(stream))
            {
                return reader.ReadAllBytes();
            }
        }


    }

    public static class Extensions
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }
}