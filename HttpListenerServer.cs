// HttpListenerServer.cs

using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LyricsComponent
{
    public class HttpListenerServer
    {
        public static HttpListenerServer? Current { get; set; }
        
        public event Action? OnLyricsReceived;
        private readonly HttpListener _listener;
        private readonly string _url;

        public string Lyrics { get; private set; }
        public string ExtraLyrics { get; private set; }
        
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
                        string json = await reader.ReadToEndAsync();
                        // 解析 JSON 数据，获取歌词内容
                        ParseLyricsFromJson(json);
                        // 触发歌词已更新事件
                        OnLyricsReceived?.Invoke();
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

        private void ParseLyricsFromJson(string json)
        {
            try
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("lyric", out JsonElement lyricElement))
                {
                    Lyrics = lyricElement.GetString()!;
                }
                if (jsonDoc.RootElement.TryGetProperty("extra", out JsonElement extraLyricElement))
                {
                    ExtraLyrics = extraLyricElement.GetString()!;
                }
            }
            catch (System.Text.Json.JsonException)
            {
            }
        }
    }
}