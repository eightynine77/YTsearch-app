using Avalonia.Controls;
using System;

namespace YTsearch.Views
{
    public partial class MainWindow : Window
    {
        // Toggle this to FALSE when you are ready to ship to users!
        private const bool IsDevelopment = true;
        private const string ProductionUrl = "https://advanced-youtube-search.vercel.app/";

        public MainWindow()
        {
            InitializeComponent();

            // We use the 'Opened' event to ensure the window is ready before loading the URL
            this.Opened += OnWindowOpened;
        }

        private void OnWindowOpened(object? sender, EventArgs e)
        {
            if (IsDevelopment)
            {
                // 1. Find a free port
                int port = YTsearch.Desktop.PortFinder.GetAvailablePort();

                // 2. Construct the localhost URL
                string devUrl = $"http://localhost:{port}";

                // 3. SHOW THE USER WHAT TO DO
                // Since C# picked the port, Vercel doesn't know about it yet.
                // We show a message box or copy to clipboard so you can run the command.
                var msg = $"Dev Mode Active.\n\n" +
                          $"1. Open your Terminal in the Vercel project folder.\n" +
                          $"2. Run this command:\n   vercel dev --listen {port}\n\n" +
                          $"The app will load {devUrl} automatically.";

                // In a real app, you might automate the terminal launch, but for now, let's just log it.
                // For simplicity in this step, let's output to Debug Console
                System.Diagnostics.Debug.WriteLine(msg);

                // Set the URL
                MainBrowser.Url = new Uri(devUrl);
            }
            else
            {
                // Production Mode
                MainBrowser.Url = new Uri(ProductionUrl);
            }
        }
    }
}