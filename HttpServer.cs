using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SolarGames.Networking
{
    public class HttpServer : IDisposable
    {
        public string Prefix { get; set; }

        HttpListener listener;

        public delegate void DOnProcessRequest(HttpListenerContext context);
        public event DOnProcessRequest OnRequest;

        bool disposed;

        public void Listen()
        {
            listener.Start();
            listener.BeginGetContext(new AsyncCallback(GetContextCallBack), null);
        }


        void GetContextCallBack(IAsyncResult ar)
        {
            if (disposed)
                return;

            HttpListenerContext context = listener.EndGetContext(ar);
            listener.BeginGetContext(new AsyncCallback(GetContextCallBack), null);

            if (OnRequest != null)
                OnRequest(context);
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            try
            {
                listener.Stop();
                listener.Close();
            }
            catch (ObjectDisposedException)
            { }
        }

        public HttpServer(string host, int port)
            : this()
        {
            if (string.IsNullOrEmpty(host))
                host = "*";

            Prefix = string.Format("http://{0}:{1}/", host, port);
            listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
        }

        public HttpServer(int port)
            : this()
        {
            Prefix = string.Format("http://*:{0}/", port);
            listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
        }

        public HttpServer()
        {
        }
    }
}
