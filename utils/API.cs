using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace ResourceMonitorAPI.utils {
    public class API {
        private HttpListener listener;
        private Thread listenerThread;
        private bool isRunning;
        public API() {
            try {
                this.listener = new HttpListener();
                this.listener.IgnoreWriteExceptions = true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                this.listener = null;
            }
        }


        public bool start() {
            this.isRunning = true;

            if (listener == null) {
                return false;
            }

            try {
                if (this.listener.IsListening) {
                    return true;
                }

                //string prefix = "https://+:443/";
                string prefix = "http://+:9001/";
                this.listener.Prefixes.Clear();
                this.listener.Prefixes.Add(prefix);
                this.listener.Start();

                if (this.listenerThread == null) {
                    this.listenerThread = new Thread(listenForRequests);
                    this.listenerThread.Start();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private void listenForRequests() {
            while (this.listener.IsListening) {
                IAsyncResult context = this.listener.BeginGetContext(new AsyncCallback(handleRequest), this.listener);
                context.AsyncWaitHandle.WaitOne();
            }
        }

        private void handleRequest(IAsyncResult result) {
            HttpListener httpListener = (HttpListener)result.AsyncState;
            if (listener == null || !listener.IsListening) {
                return;
            }

            HttpListenerContext httpContext;
            try {
                httpContext = httpListener.EndGetContext(result);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return;
            }

            HttpListenerRequest httpRequest = httpContext.Request;
            string requestString = httpRequest.RawUrl.Substring(1);
            Console.WriteLine(requestString);

            string httpBody = "";
            using (Stream bodyStream = httpRequest.InputStream) {
                using (StreamReader streamReader = new StreamReader(bodyStream, httpRequest.ContentEncoding)) {
                    httpBody = streamReader.ReadToEnd();
                }
            }

            string[] keys;
            try {
                keys = requestString.Split('/');
            }
            catch {
                keys = new string[1];
                keys[0] = requestString;
            }

            string json = "";
        }

        private void sendJson(HttpListenerResponse response, string content) {
            if (content == null) {
                content = "{}";
            }
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);

            response.AddHeader(HttpWrapper.Headers.CACHE_CONTROL, HttpWrapper.HeaderValues.CacheControl.NO_CACHE);
            response.AddHeader(HttpWrapper.Headers.ACCESS_CONTROL_ALLOW_ORIGIN, HttpWrapper.HeaderValues.AccessControlAllowOrigin.ANY_ORIGIN);
            response.ContentLength64 = contentBytes.Length;
            response.ContentType = HttpWrapper.ContentTypes.JSON;

            try {
                Stream outputStream = response.OutputStream;
                outputStream.Write(contentBytes, 0, contentBytes.Length);
                outputStream.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                /*
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                Console.WriteLine(String.Format("Exception on line: {0}", line));
                */
            }

            response.Close();
        } 
    }
}