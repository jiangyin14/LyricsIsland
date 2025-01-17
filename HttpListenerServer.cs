// HttpListenerServer.cs

using System;
using System.Net;
using System.IO;
using System.Text;

namespace LyricsComponent
{
    public class HttpListenerServer
    {
        private readonly HttpListener _listener;
        private readonly string _url;

        public HttpListenerServer(string url)
        {
            _url = url;
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"Listening on {_url}");
            Listen();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private async void Listen()
        {
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.LocalPath == "/component/lyrics/lyrics/")
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var json = await reader.ReadToEndAsync();
                        // 解析 JSON 数据，获取一言内容
                        var lyrics = ParseLyricsFromJson(json);
                        // 更新界面上的一言内容
                        UpdateLyricsOnUI(lyrics);
                    }

                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/plain";
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        await writer.WriteAsync("Lyrics updated successfully!");
                    }
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                response.Close();
            }
        }

        private string ParseLyricsFromJson(string json)
        {
            // 解析 JSON 数据的逻辑
            // 这里只是一个示例，您需要根据实际的 JSON 格式来解析
            return json;
        }

        private void UpdateLyricsOnUI(string lyrics)
        {
            // 更新界面上的一言内容的逻辑
            // 这里只是一个示例，您需要根据实际的 UI 框架来更新
        }
    }
}