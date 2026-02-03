using System.Net;
using System.Net.Sockets;

namespace YTsearch.Desktop
{
    public static class PortFinder
    {
        public static int GetAvailablePort()
        {
            // We create a TcpListener on IP 0.0.0.0 (Loopback) with port 0.
            // "Port 0" tells the OS to assign us any available dynamic port automatically.
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                // Get the port the OS assigned to us
                int port = ((IPEndPoint)socket.LocalEndPoint!).Port;

                // Close the socket immediately so the port is freed up for our app to use
                return port;
            }
        }
    }
}