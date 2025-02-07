using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;

namespace LyricsComponent
{
    [ComponentInfo(
        "8214141d-6b3b-4ad3-9674-389945493492",
        "歌词(第二行)",
        PackIconKind.Music,
        "在主界面上显示来自音乐软件的歌词。"
    )]
    public partial class ExtraLyricsControl : ComponentBase, IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly HttpListener _listener;
        private readonly string _url = "http://127.0.0.1:50063/";
        private bool _isListening = false;

        public ExtraLyricsControl()
        {
            InitializeComponent();
            LoadLyricAsync();
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url);
            StartHttpListener();
        }

        /// <summary>
        /// 初始化并启动 HTTP 监听器。
        /// </summary>
        private void StartHttpListener()
        {
            try
            {
                _listener.Start();
                _isListening = true;
                Console.WriteLine($"HTTP 监听器已启动，正在监听 {_url}");
                ListenAsync();
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"启动 HTTP 监听器失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 异步监听传入的 HTTP 请求。
        /// </summary>
        private async void ListenAsync()
        {
            while (_isListening && _listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (HttpListenerException ex) when (ex.ErrorCode == 995) // 操作已中止。
                {
                    // 监听器已停止，无需处理。
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"监听过程中发生错误: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 处理传入的 HTTP 请求。
        /// </summary>
        /// <param name="context">HTTP 上下文。</param>
        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            Console.WriteLine($"收到请求: {request.HttpMethod} {request.Url}");

            if (request.HttpMethod == "POST" && request.Url.LocalPath == "/component/lyrics/lyrics/")
            {
                try
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var json = await reader.ReadToEndAsync();
                        var lyric = ParseLyricFromJson(json);
                        UpdateLyricOnUI(lyric);
                        Console.WriteLine("歌词已更新。");
                    }

                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/plain";
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        await writer.WriteAsync("歌词更新成功！");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"处理请求时出错: {ex.Message}");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        await writer.WriteAsync("内部服务器错误");
                    }
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync("未找到请求的资源");
                }
            }

            response.Close();
        }

        /// <summary>
        /// 从接收到的 JSON 中解析出“一言”。
        /// 根据实际的 JSON 结构调整此方法。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <returns>解析出的一言。</returns>
        private string ParseLyricFromJson(string json)
        {
            // 示例 JSON: { "lyric": "你的歌词"}
            try
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("extra", out var lyricElement))
                {
                    return lyricElement.GetString();
                }
                return "未解析到歌词";
            }
            catch (System.Text.Json.JsonException)
            {
                return "输入解析错误";
            }
        }

        /// <summary>
        /// 在 UI 上更新显示的一言内容。
        /// </summary>
        /// <param name="lyric">要显示的一言。</param>
        private void UpdateLyricOnUI(string lyric)
        {
            Dispatcher.Invoke(() => LyricsText.Text = lyric);
        }

        /// <summary>
        /// 从外部 API 加载初始的一言。
        /// </summary>
        private async void LoadLyricAsync()
        {
            try
            {
                UpdateLyricOnUI("等待音乐软件侧传输歌词...");
            }
            catch (HttpRequestException)
            {
                UpdateLyricOnUI("加载失败");
            }
        }

        /// <summary>
        /// 释放资源并停止 HTTP 监听器。
        /// </summary>
        public void Dispose()
        {
            _isListening = false;
            if (_listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
                Console.WriteLine("HTTP 监听器已停止。");
            }
            _httpClient.Dispose();
        }

        /// <summary>
        /// 当控件从 UI 卸载时，确保资源被释放。
        /// </summary>
        /// <param name="e">事件参数。</param>
        // protected override void OnUnloaded(RoutedEventArgs e)
        // {
        //     base.OnUnloaded(e);
        //     Dispose();
        // }
    }
}
