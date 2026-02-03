using Avalonia.Controls;
using System;

namespace YTsearch.Views
{
    public partial class MainWindow : Window
    {
        private LocalServer _server;

        public MainWindow()
        {
            InitializeComponent();

            // Start the local server
            _server = new LocalServer();
            _server.Start();

            // Point the WebView to our local embedded site
            // The script.js calls "/api/...", which LocalServer automatically forwards to Vercel
            this.Opened += (s, e) =>
            {
                MainBrowser.Url = new Uri($"http://localhost:{_server.Port}/index.html");
            };
        }
    }
}