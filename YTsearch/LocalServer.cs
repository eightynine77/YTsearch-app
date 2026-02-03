using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YTsearch
{
    public class LocalServer
    {
        private HttpListener _listener;
        private string _baseDomain = "https://advanced-youtube-search.vercel.app"; // Your real site
        public int Port { get; private set; }

        public void Start()
        {
            // 1. Find a free port automatically
            Port = GetAvailablePort();

            // 2. Start the internal web server
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{Port}/");
            _listener.Start();

            // 3. Listen for requests in the background
            Task.Run(ListenLoop);
        }

        private void ListenLoop()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ProcessRequest(context);
                }
                catch { break; } // Server stopped
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            string rawPath = context.Request.Url!.AbsolutePath;
            string path = rawPath.TrimStart('/');

            // A. API PROXY: If the JS asks for /api/..., forward it to Vercel
            if (rawPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                ProxyApiRequest(context, rawPath + context.Request.Url.Query);
                return;
            }

            // B. FILE SERVER: Serve embedded HTML/CSS/JS
            if (string.IsNullOrEmpty(path)) path = "index.html";

            // Convert web path (css/style.css) to Assembly Resource ID (YTsearch.wwwroot.css.style.css)
            var assembly = Assembly.GetExecutingAssembly();
            // Note: Resource names use dots instead of slashes
            string resourceName = $"YTsearch.wwwroot.{path.Replace('/', '.')}";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    // Map extensions to MIME types
                    if (path.EndsWith(".html")) context.Response.ContentType = "text/html";
                    else if (path.EndsWith(".js")) context.Response.ContentType = "text/javascript";
                    else if (path.EndsWith(".css")) context.Response.ContentType = "text/css";

                    stream.CopyTo(context.Response.OutputStream);
                    context.Response.StatusCode = 200;
                }
                else
                {
                    // 404 Not Found
                    byte[] notFound = Encoding.UTF8.GetBytes("Resource not found: " + resourceName);
                    context.Response.OutputStream.Write(notFound, 0, notFound.Length);
                    context.Response.StatusCode = 404;
                }
            }
            context.Response.Close();
        }

        private void ProxyApiRequest(HttpListenerContext context, string apiPath)
        {
            try
            {
                // Fetch data from REAL Vercel site
                using (var client = new System.Net.Http.HttpClient())
                {
                    // Example: https://advanced-youtube-search.vercel.app/api/search?q=...
                    var response = client.GetAsync(_baseDomain + apiPath).Result;
                    var content = response.Content.ReadAsByteArrayAsync().Result;

                    // Forward headers and content back to the app
                    context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
                    context.Response.StatusCode = (int)response.StatusCode;
                    context.Response.OutputStream.Write(content, 0, content.Length);
                }
            }
            catch (Exception ex)
            {
                byte[] error = Encoding.UTF8.GetBytes("Proxy Error: " + ex.Message);
                context.Response.StatusCode = 500;
                context.Response.OutputStream.Write(error, 0, error.Length);
            }
            context.Response.Close();
        }

        private int GetAvailablePort()
        {
            using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                return ((IPEndPoint)socket.LocalEndPoint!).Port;
            }
        }
    }
}