
#if NETSTANDARD1_4
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    internal class WebClient : IDisposable
    {
        public WebClient()
        {

        }

        public void Dispose()
        {
        }

        public void DownloadFile(string url, string filename)
        {
            DownloadFileAsync(new Uri(url), filename).RunSynchronously();
        }

        public async Task DownloadFileAsync(Uri requestUri, string filename)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    using (Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }
    }
}
#endif