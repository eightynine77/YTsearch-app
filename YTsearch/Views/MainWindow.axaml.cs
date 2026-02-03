using Avalonia.Controls;
using YTsearch;
using Avalonia.Interactivity;
using System;

namespace YTsearch.Views
{
    public partial class MainWindow : Window
    {
        private LocalServer _server;

        public MainWindow()
        {
            InitializeComponent();

            // 1. Start the C# Local Server
            _server = new LocalServer();
            _server.Start();

            // 2. Load the embedded index.html
            // The browser will load http://localhost:PORT/index.html
            this.Opened += (s, e) =>
            {
                if (MainBrowser != null)
                {
                    MainBrowser.Url = new Uri($"http://localhost:{_server.Port}/index.html");
                }
            };
        }
    }
}